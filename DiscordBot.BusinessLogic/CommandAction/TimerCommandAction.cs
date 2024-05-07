using Discord;
using Discord.WebSocket;
using DiscordBot.BusinessLogic.Extensions;
using DiscordBot.Storage.Database;
//TODO: Включить обратно
namespace DiscordBot.BusinessLogic.CommandAction
{
    public class TimerCommandAction
    {
        private readonly IGuildDataProvider guildDataProvider;

        public TimerCommandAction(IGuildDataProvider guildDataProvider)
        {
            this.guildDataProvider = guildDataProvider;
        }

        private class CoffeeBrakeTimer : IDisposable
        {
            public string Id { get; private set; }
            private Timer timer;

            public CoffeeBrakeTimer(long timerDelay, Action callback)
            {
                timer = new Timer(TimerCallback, callback, timerDelay, Timeout.Infinite);
                Id = Guid.NewGuid().ToString().Substring(0, 8);
            }

            private void TimerCallback(object? state)
            {
                ((Action)state!)?.Invoke();
            }

            public void Dispose()
            {
                timer.Dispose();
            }
        }

        private List<CoffeeBrakeTimer> coffebrakeTimers = new List<CoffeeBrakeTimer>();

        public List<(string command, string description)> GetDescription()
        {
            return new List<(string command, string description)>
                { ("CoffeeBrake, cb", "CBCommandDesc"), ("Timers, tm", "") };
        }

        public List<(string command, Action<SocketMessage> action)> GetAvailableCommands()
        {
            return new List<(string command, Action<SocketMessage>)>
                { ("coffeebrake", CreateCoffebrakeTimer), ("cb", CreateCoffebrakeTimer) };
        }

        public async void CreateCoffebrakeTimer(SocketMessage input)
        {
            ulong id = ((input.Channel as SocketGuildChannel)!).Guild.Id;
            List<string> data = StringParserHelper.GetCommandData(input.Content);
            if (data.Count <= 0)
                input.SendLocalizedMessage("CBInputError", guildDataProvider.GetGuildCulture(id));
            string numberString = data[0];
            string[] parsedTime = numberString.Split(':', ',', '.');
            if (parsedTime.Length < 2)
            {
                input.SendLocalizedMessage("ParsingError", guildDataProvider.GetGuildCulture(id));
                return;
            }

            int[] timeArray = new int[2];
            for (int i = 0; i < 2; i++)
            {
                if (int.TryParse(parsedTime[i], out int parsed))
                {
                    timeArray[i] = parsed;
                }
                else
                {
                    input.SendLocalizedMessage("ParsingError", guildDataProvider.GetGuildCulture(id));
                    return;
                }
            }

            DateTime targetTime = DateTime.Parse($"{timeArray[0]}:{timeArray[1]}");
            var current = DateTime.UtcNow + new TimeSpan(5, 0, 0);
            var timeToGo = targetTime.TimeOfDay - current.TimeOfDay;
            long dueTime = Math.Max(0L, (long)timeToGo.TotalMilliseconds);
            IUserMessage responseMessage = await input.SendLocalizedMessage("CBCommandTimerSetted",
                guildDataProvider.GetGuildCulture(id),
                new object[] { $"{timeToGo:%h}", $"{timeToGo:%m}" });
            await responseMessage.AddReactionAsync(new Emoji("\U00002705"));
            CoffeeBrakeTimer timer = new CoffeeBrakeTimer(dueTime, () => TimerIsDue(responseMessage));
            coffebrakeTimers.Add(timer);
        }

        private async void TimerIsDue(IUserMessage answer)
        {
            var message = await answer.Channel.GetMessageAsync(answer.Id, CacheMode.AllowDownload);
            var reactions = message.Reactions.Keys;
            var users = reactions
                .Select(async emote => await message.GetReactionUsersAsync(emote, 100).ToListAsync())
                .SelectMany(user => user.Result)
                .SelectMany(u => u)
                .Where(user => !user.IsBot)
                .Select(user => user.Mention)
                .Distinct()
                .ToList();
            var mentionStr = string.Join(",", users);
            await answer.Channel.SendLocalizedMessage("CBCommandTimerIsDue",
                guildDataProvider.GetGuildCulture((answer.Channel as SocketGuildChannel)!.Guild.Id),
                new[] { mentionStr });
        }
    }
}