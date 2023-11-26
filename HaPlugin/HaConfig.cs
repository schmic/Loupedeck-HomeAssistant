namespace Loupedeck.HomeAssistant
{
    using System;
    using System.IO;

    public class HaConfig
    {
        public String Url;

        public String Token;

        public static HaConfig Read()
        {
            var DEFAULT_PATH = Path.Combine(".loupedeck", "homeassistant");
            var UserProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var ConfigFile = Path.Combine(UserProfilePath, DEFAULT_PATH, "homeassistant.json");

            if (!IoHelpers.FileExists(ConfigFile))
            {
                PluginLog.Error($"Configuration file is missing or unreadable.");
                return null;
            }

            var Config = JsonHelpers.DeserializeAnyObjectFromFile<HaConfig>(ConfigFile);

            return Config;
        }
    }
}
