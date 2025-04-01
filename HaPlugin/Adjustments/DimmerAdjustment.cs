namespace Loupedeck.HaPlugin.Adjustments
{
    using System;
    using System.Collections.Generic;

    using Loupedeck.HaPlugin;
    using Loupedeck.HaPlugin.Helpers;
    using Loupedeck.HaPlugin.Json;

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

        protected override String GetCommandDisplayName(String entity_id, PluginImageSize imageSize)
        {
            if (entity_id.IsNullOrEmpty())
            { return null; }

            var entityState = this.plugin.States[entity_id];
            return $"{entityState.FriendlyName}";
        }

        protected override String GetAdjustmentDisplayName(String entity_id, PluginImageSize imageSize)
        {
            if (entity_id.IsNullOrEmpty())
            { return null; }

            var entityState = this.plugin.States[entity_id];
            return $"{entityState.FriendlyName}";
        }

        protected override String GetAdjustmentValue(String entity_id)
        {
            if (entity_id.IsNullOrEmpty())
            { return null; }

            var entityState = this.plugin.States[entity_id];
            var entityValue = AsInt(entityState, "brightness");
            return entityValue.ToString();
        }

        protected override void ApplyAdjustment(String entity_id, Int32 value)
        {
            if (entity_id.IsNullOrEmpty())
            { return; }

            var entityState = this.plugin.States[entity_id];
            var entityValue = AsInt(entityState, "brightness");
            var brightness = entityValue + value;

            PluginLog.Verbose($"{entity_id} brightness {entityValue} => {brightness}");

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

        private static Int32 AsInt(HaState entityState, String attributeName)
        {
            var entityValueF = Convert.ToSingle(entityState.Attributes[attributeName].ToString());
            var entityValue = Convert.ToInt32(entityValueF);
            return entityValue;
        }
    }
}
