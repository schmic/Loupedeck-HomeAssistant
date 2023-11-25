namespace Loupedeck.HomeAssistant.Json
{
    using System;

    using Newtonsoft.Json.Linq;

    public class HaState
    {
        public String Entity_Id { get; set; }

        public String State { get; set; }

        public JObject Attributes { get; set; }

        public String FriendlyName => this.Attributes["friendly_name"]?.ToString();

        public String Icon => this.Attributes["icon"]?.ToString();

        public override String ToString() => this.Entity_Id + " // " + this.State + " // " + this.Attributes;
    }
}
