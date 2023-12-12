namespace Loupedeck.HomeAssistant.Adjustments
{
    using System;
    using System.Collections.Generic;

    using Loupedeck.HomeAssistant.Json;

    using Newtonsoft.Json.Linq;

    public class DimmerAdjustment : PluginDynamicAdjustment
    {
        private HaPlugin plugin;

        public DimmerAdjustment() : base(true)
        {
            this.GroupName = "Dimmer";
            this.ResetDisplayName = "Toggle";
        }

        protected override Boolean OnLoad()
        {
            this.plugin = base.Plugin as HaPlugin;

            this.plugin.StatesReady += (sender, e) =>
            {
                PluginLog.Verbose($"{this.GroupName}Command.OnLoad() => StatesReady");

                foreach (KeyValuePair<String, Json.HaState> group in this.plugin.States)
                {
                    var state = group.Value;
                    if (this.IsDimmer(state))
                    {
                        this.AddParameter(state.Entity_Id, state.FriendlyName, "Dimmer");
                    }
                }

                PluginLog.Info($"[group: {this.GroupName}] [count: {this.GetParameters().Length}]");
            };

            this.plugin.StateChanged += (sender, e) => this.ActionImageChanged(e.Entity_Id);

            return true;
        }

        private Boolean IsDimmer(HaState state) => state.Entity_Id.StartsWith("light.") && state.Attributes.ContainsKey("brightness");

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            if (actionParameter.IsNullOrEmpty())
            { return null; }

            var entityState = this.plugin.States[actionParameter];
            return $"{entityState.FriendlyName}";
        }

        protected override String GetAdjustmentDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            if (actionParameter.IsNullOrEmpty())
            { return null; }

            var entityState = this.plugin.States[actionParameter];
            return $"{entityState.FriendlyName}";
        }

        protected override String GetAdjustmentValue(String actionParameter)
        {
            if (actionParameter.IsNullOrEmpty())
            { return null; }

            var entityState = this.plugin.States[actionParameter];
            Int32.TryParse(entityState?.Attributes["brightness"]?.ToString(), out var entityValue);

            return entityValue.ToString();
        }

        protected override void ApplyAdjustment(String entity_id, Int32 value)
        {
            if (entity_id.IsNullOrEmpty())
            { return; }

            var entityState = this.plugin.States[entity_id];
            Int32.TryParse(entityState.Attributes["brightness"]?.ToString(), out var entityValue);
            var brightness = entityValue + value;

            PluginLog.Verbose($"{entity_id} {entityValue} => {brightness}");

            this.plugin.States[entity_id].Attributes["brightness"] = brightness.ToString();

            var data = new JObject {
                { "domain", "light" },
                { "service", "turn_on" },
                { "service_data", new JObject { { "brightness", brightness } } },
                { "target", new JObject { { "entity_id", entity_id } } }
            };

            this.plugin.CallService(data);
        }

        protected override void RunCommand(String entity_id)
        {
            var data = new JObject {
                { "domain", "light" },
                { "service", "toggle" },
                { "target", new JObject { { "entity_id", entity_id } } }
            };

            this.plugin.CallService(data);
        }
    }
}
