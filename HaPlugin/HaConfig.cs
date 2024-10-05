namespace Loupedeck.HaPlugin
{
    using System;
    using System.IO;

    using Loupedeck.HaPlugin;
    using Loupedeck.HaPlugin.Helpers;

    public class HaConfig
    {
        public String Url { get; set; }

        public Uri ApiUrl
        {
            get
            {
                var builder = new UriBuilder(this.Url.Replace("http", "ws"))
                {
                    Path = "/api/websocket",
                };
                return builder.Uri;
            }
        }

        public String Token;

        public static HaConfig FromString(String jsonConfig) => JsonHelpers.DeserializeAnyObject<HaConfig>(jsonConfig);

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
