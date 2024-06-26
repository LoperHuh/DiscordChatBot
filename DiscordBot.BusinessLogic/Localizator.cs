﻿using System.Globalization;
using System.Resources;

namespace DiscordBot.BusinessLogic;

public class Localizator
{
    public Localizator(CultureInfo culture)
    {
        _currentCulture = culture;
        _resourceManager = new ResourceManager(typeof(Resources.CommandAction.Resources));
        _resourceSet = _resourceManager.GetResourceSet(culture, true, true);
    }

    private ResourceSet _resourceSet;
    private readonly ResourceManager _resourceManager;
    private CultureInfo _currentCulture;

    public string Localize(string key)
    {
        return GetString(key, _currentCulture);
    }

    // God i love chat GPT. All below generated by him/her... they?
    private string GetString(string key, CultureInfo culture)
    {
        // Try to get the localized string for the specified key and culture.

        var value = _resourceSet?.GetString(key);
        // If the localized string is not found and the current culture is not
        // the default culture, try to get the localized string from the default
        // culture's resource file.
        if (value == null && culture != CultureInfo.InvariantCulture)
        {
            var defaultCulture = CultureInfo.GetCultureInfo("en-US");
            _resourceSet = _resourceManager.GetResourceSet(defaultCulture, true, true);
            value = _resourceSet?.GetString(key);
        }

        // If the localized string is still not found, return the key itself.
        return value ?? key;
    }
}