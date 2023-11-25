namespace Loupedeck.HomeAssistant.Json
{
    using System;

    using Newtonsoft.Json.Linq;

    public class HaEvent
    {
        public JObject new_state
        { set => this.State = value.ToObject<HaState>(); }

        public HaState State;
    }
}
