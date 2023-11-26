namespace Loupedeck.HomeAssistant
{
    using System;
    using System.Collections.Generic;

    public class SwitchCommand : PluginDynamicCommand
    {
        private HaPlugin plugin;

        public SwitchCommand() : base()
        {
        }

        protected override Boolean OnLoad()
        {
            this.plugin = base.Plugin as HaPlugin;

            this.plugin.StatesReady += (sender, e) =>
            {
                PluginLog.Verbose($"SwitchCommand.OnLoad() => StatesReady");
                foreach (KeyValuePair<String, Json.HaState> group in this.plugin.States)
                {
                    var state = group.Value;
                    if (state.Entity_Id.StartsWith("switch."))
                    {
                        this.AddParameter(state.Entity_Id, state.FriendlyName, "Switch");
                    }
                }
            };

            this.plugin.StateChanged += (sender, e) => this.ActionImageChanged(e.Entity_Id);

            return true;
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            if (actionParameter.IsNullOrEmpty())
            { return ""; }

            var entityState = this.plugin.States[actionParameter];
            return $"{entityState.State} {entityState.FriendlyName}";
        }

        protected override void RunCommand(String actionParameter) => this.plugin.SwitchToggle(actionParameter);
    }
}
