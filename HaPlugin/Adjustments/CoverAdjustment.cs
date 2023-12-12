namespace Loupedeck.HomeAssistant.Adjustments
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    public class CoverAdjustment : PluginDynamicAdjustment
    {
        private HaPlugin plugin;

        public CoverAdjustment() : base(true)
        {
            this.GroupName = "Cover";
            this.ResetDisplayName = "Toggle"; // FIXME: does not apply
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
                    if (state.Entity_Id.StartsWith("cover."))
                    {
                        this.AddParameter(state.Entity_Id, state.FriendlyName, this.GroupName);
                    }
                }

                PluginLog.Info($"[group: {this.GroupName}] [count: {this.GetParameters().Length}]");
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
            Int32.TryParse(entityState.Attributes["current_position"]?.ToString(), out var entityValue);

            return entityValue.ToString();
        }

        protected override void ApplyAdjustment(String entity_id, Int32 value)
        {
            if (entity_id.IsNullOrEmpty())
            { return; }

            var entityState = this.plugin.States[entity_id];
            PluginLog.Verbose(entityState.ToString());
            Int32.TryParse(entityState.Attributes["current_position"]?.ToString(), out var entityValue);
            var position = entityValue + value;

            this.plugin.States[entity_id].Attributes["current_position"] = position.ToString();

            var reqData = new JObject {
                { "domain", "cover" },
                { "service", "set_cover_position" },
                { "service_data", new JObject { { "position", position} } },
                { "target", new JObject { { "entity_id", entity_id } } }
            };

            this.plugin.CallService(reqData);
        }

        protected override void RunCommand(String entity_id)
        {
            var reqData = new JObject {
                { "domain", "cover" },
                { "service", "toggle" },
                { "service_data", new JObject { } },
                { "target", new JObject { { "entity_id", entity_id } } }
            };

            this.plugin.CallService(reqData);
        }
    }
}
