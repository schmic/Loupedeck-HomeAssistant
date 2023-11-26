namespace Loupedeck.HomeAssistant
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class ClimateAdjustment : PluginDynamicAdjustment
    {
        private HaPlugin plugin;

        public ClimateAdjustment() : base(true)
        {
        }

        protected override Boolean OnLoad()
        {
            this.plugin = base.Plugin as HaPlugin;

            this.plugin.StatesReady += (sender, e) =>
            {
                PluginLog.Verbose($"ClimateAdjustment.OnLoad() => StatesReady");
                foreach (KeyValuePair<String, Json.HaState> group in this.plugin.States)
                {
                    var state = group.Value;
                    if (state.Entity_Id.StartsWith("climate."))
                    {
                        //PluginLog.Verbose($"climate: {state}");
                        this.AddParameter(state.Entity_Id, state.FriendlyName, "Climate");
                    }
                }
            };

            this.plugin.StateChanged += (sender, e) => this.ActionImageChanged(e.Entity_Id);

            return true;
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            if (actionParameter.IsNullOrEmpty())
            { return null; }

            var entityState = this.plugin.States[actionParameter];
            return $"{entityState.State} {entityState.FriendlyName}";
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
            Int32.TryParse(entityState.Attributes["temperature"]?.ToString(), out var entityValue);

            return entityValue.ToString();
        }

        protected override void ApplyAdjustment(String entity_id, Int32 value)
        {
            if (entity_id.IsNullOrEmpty())
            { return; }

            var entityState = this.plugin.States[entity_id];
            PluginLog.Verbose(entityState.ToString());
            Int32.TryParse(entityState.Attributes["temperature"]?.ToString(), out var entityValue);
            var newEntityValue = entityValue + value;

            PluginLog.Verbose($"{entity_id} {entityValue} => {newEntityValue}");
            this.plugin.States[entity_id].Attributes["temperature"] = newEntityValue.ToString();

            this.plugin.ClimateTemperature(entity_id, newEntityValue);
        }

        protected override void RunCommand(String entity_id) => this.plugin.ClimateTemperature(entity_id, 18);
    }
}
