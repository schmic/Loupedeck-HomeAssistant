namespace Loupedeck.HomeAssistant
{
    using System;
    using System.Collections.Generic;

    public class LightCommand : PluginDynamicCommand
    {
        private HaPlugin plugin;

        public LightCommand() : base()
        {
        }

        protected override Boolean OnLoad()
        {
            PluginLog.Verbose($"LightCommand.OnLoad()");
            this.plugin = base.Plugin as HaPlugin;

            this.plugin.StatesReady += (sender, e) =>
            {
                PluginLog.Verbose($"LightCommand.OnLoad() => StatesReady");
                foreach (KeyValuePair<String, Json.HaState> group in this.plugin.States)
                {
                    var state = group.Value;
                    if (state.Entity_Id.StartsWith("light."))
                    {
                        PluginLog.Verbose($"{state.Entity_Id}");
                        this.AddParameter(state.Entity_Id, state.FriendlyName, "Lights");
                    }
                }
            };

            this.plugin.StateChanged += (sender, e) => this.ActionImageChanged(e.Entity_Id);

            return true;
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            if (actionParameter.IsNullOrEmpty())
            {
                return "";

            }

            var entityState = this.plugin.States[actionParameter];
            return $"{entityState.State} {entityState.FriendlyName}";
        }

        protected override void RunCommand(String actionParameter)
        {
            this.plugin.LightToggle(actionParameter);
            this.ActionImageChanged(actionParameter);
        }
    }
}
