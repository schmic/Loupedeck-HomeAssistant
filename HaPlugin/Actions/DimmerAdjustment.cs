namespace Loupedeck.HomeAssistant
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class DimmerAdjustment : PluginDynamicAdjustment
    {
        private HaPlugin plugin;

        public DimmerAdjustment() : base(true)
        {
        }

        protected override Boolean OnLoad()
        {
            this.plugin = base.Plugin as HaPlugin;

            this.plugin.StatesReady += (sender, e) =>
            {
                PluginLog.Verbose($"DimmerAdjustment.OnLoad() => StatesReady");
                foreach (KeyValuePair<String, Json.HaState> group in this.plugin.States)
                {
                    var state = group.Value;
                    if (state.Entity_Id.StartsWith("light."))
                    {
                        // TODO: filter supported_color_modes["brightness"]
                        this.AddParameter(state.Entity_Id, state.FriendlyName, "Dimmer");
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
            Int32.TryParse(entityState?.Attributes["brightness"]?.ToString(), out var entityValue);

            return entityValue.ToString();
        }

        protected override void ApplyAdjustment(String entity_id, Int32 value)
        {
            if (entity_id.IsNullOrEmpty())
            { return; }

            var entityState = this.plugin.States[entity_id];
            Int32.TryParse(entityState.Attributes["brightness"]?.ToString(), out var entityValue);
            var newEntityValue = entityValue + value;

            PluginLog.Verbose($"{entity_id} {entityValue} => {newEntityValue}");

            this.plugin.States[entity_id].Attributes["brightness"] = newEntityValue.ToString();

            this.plugin.LightBrightness(entity_id, newEntityValue);
        }

        protected override void RunCommand(String actionParameter) => this.plugin.LightToggle(actionParameter);
    }
}
