namespace DiscordBot.BusinessLogic.Extensions
{
    public struct CommandContent
    {
        public CommandContent()
        {
            Command = string.Empty;
            Content = string.Empty;
        }

        public CommandContent(string command, string content)
        {
            Command = command;
            Content = content;
        }

        public string Command { get; }
        public string Content { get; }
    }

    // TODO: string operation is expensive as hell. Convert to StringBuilder?
    public static class StringParserHelper
    {
        private static string commandCharacter = "!";

        public static bool IsCommand(string message)
        {
            return message.Substring(0, 1) == commandCharacter;
        }

        public static string GetCommand(string message)
        {
            string[] splitted = message.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (splitted.Length <= 0)
                return String.Empty;
            return splitted[0].Substring(1).ToLower();
        }

        public static List<string> GetCommandData(string message)
        {
            string[] splitted = message.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            List<string> output = new List<string>();
            for (int i = 1; i < splitted.Length; i++)
            {
                output.Add(splitted[i]);
            }

            return output;
        }

        public static CommandContent ParseMessage(string message)
        {
            string[] splitted = message.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (splitted.Length <= 0)
                return new CommandContent();
            var commandString = splitted[0];
            int index = message.IndexOf(commandString, StringComparison.Ordinal);
            string commandParams = (index < 0)
                ? message
                : message.Remove(index, commandString.Length);
            commandString = commandString.Replace(commandCharacter, "");
            return new CommandContent(commandString, commandParams);
        }

        public static IEnumerable<string> Split(string str, int chunkSize)
        {
            for (int i = 0; i < str.Length; i += chunkSize)
                yield return str.Substring(i, Math.Min(chunkSize, str.Length - i));
        }
    }
}