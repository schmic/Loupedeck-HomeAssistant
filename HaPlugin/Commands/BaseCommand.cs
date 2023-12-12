namespace Loupedeck.HomeAssistant.Commands
{
    using System;
    using System.Collections.Generic;

    using Loupedeck.HomeAssistant.Events;

    public abstract class BaseCommand : PluginDynamicCommand
    {
        protected BaseCommand(String groupName) : base() => this.GroupName = groupName;

        protected abstract Boolean EntitiyFilter(String entity_id);

        protected abstract override void RunCommand(String entity_id);

        protected override Boolean OnLoad()
        {
            using (var plugin = base.Plugin as HaPlugin)
            {
                plugin.StatesReady += this.StatesReady;
                plugin.StateChanged += this.StateChanged;
            }

            return true;
        }

        protected override Boolean OnUnload()
        {
            using (var plugin = base.Plugin as HaPlugin)
            {
                plugin.StateChanged -= this.StateChanged;
                plugin.StatesReady -= this.StatesReady;
            }

            return true;
        }

        protected HaPlugin GetPlugin() => (HaPlugin)base.Plugin;

        protected Dictionary<String, Json.HaState> GetStates() => this.GetPlugin().States;
 
        private void StatesReady(Object sender, EventArgs e)
        {
            PluginLog.Verbose($"{this.GroupName}Command.OnLoad() => StatesReady");

            foreach (KeyValuePair<String, Json.HaState> kvp in this.GetStates())
            {
                if (!this.EntitiyFilter(kvp.Key))
                { continue; }

                var state = kvp.Value;
                this.AddParameter(state.Entity_Id, state.FriendlyName, this.GroupName);
            }

            PluginLog.Info($"[group: {this.GroupName}] [count: {this.GetParameters().Length}]");
        }

        private void StateChanged(Object sender, StateChangedEventArgs e)
        {
            if (!this.EntitiyFilter(e.Entity_Id))
            { return; }

            this.ActionImageChanged(e.Entity_Id);
        }

        protected override String GetCommandDisplayName(String entity_id, PluginImageSize imageSize)
        {
            if (entity_id.IsNullOrEmpty())
            { return base.GetCommandDisplayName(entity_id, imageSize); }

            var states = this.GetStates();

            var FriendlyName = states[entity_id].FriendlyName;
            var State = states[entity_id].State;
            var Unit_Of_Measurement = states[entity_id].Attributes["unit_of_measurement"];

            return $"{FriendlyName}\n{State}{Unit_Of_Measurement}";
        }
    }
}
