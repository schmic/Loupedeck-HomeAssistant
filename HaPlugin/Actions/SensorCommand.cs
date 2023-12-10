namespace Loupedeck.HomeAssistant
{
    using System;
    using System.Collections.Generic;

    using Loupedeck.HomeAssistant.Events;

    public class SensorCommand : PluginDynamicCommand
    {
        protected BitmapColor BgColor_off = new BitmapColor(22, 33, 38);
        protected BitmapColor BgColor_on = new BitmapColor(73, 27, 74);

        protected HaPlugin plugin;

        public SensorCommand() : base() { }

        protected override Boolean OnLoad()
        {
            this.plugin = (HaPlugin)base.Plugin;

            this.plugin.StatesReady += this.StatesReady;
            this.plugin.StateChanged += this.StateChanged;

            return true;
        }

        protected override Boolean OnUnload()
        {
            this.plugin.StateChanged -= this.StateChanged;
            this.plugin.StatesReady -= this.StatesReady;

            return true;
        }

        private void StatesReady(Object sender, EventArgs e)
        {
            PluginLog.Verbose($"SensorCommand.OnLoad() => StatesReady");

            foreach (KeyValuePair<String, Json.HaState> kvp in this.plugin.States)
            {
                if (!kvp.Key.StartsWith($"sensor.") && !kvp.Key.StartsWith($"binary_sensor."))
                { continue; }

                PluginLog.Verbose($"{kvp.Key} is a sensor with {kvp.Value}");


                var state = kvp.Value;
                this.AddParameter(state.Entity_Id, state.FriendlyName, "Status");
            }

            PluginLog.Info($"Sensor => {this.GetParameters().Length} found.");
        }

        private void StateChanged(Object sender, StateChangedEventArgs e)
        {
            if (!e.Entity_Id.StartsWith($"sensor.") && !e.Entity_Id.StartsWith($"binary_sensor."))
            { return; }
            
            this.ActionImageChanged(e.Entity_Id);
        }

        protected override String GetCommandDisplayName(String entity_id, PluginImageSize imageSize)
        {
            if (entity_id.IsNullOrEmpty())
            { return base.GetCommandDisplayName(entity_id, imageSize); }

            var FriendlyName = this.plugin.States[entity_id].FriendlyName;
            var State = this.plugin.States[entity_id].State;
            var Unit_Of_Measurement = this.plugin.States[entity_id].Attributes["unit_of_measurement"];

            return $"{FriendlyName}\n{State}{Unit_Of_Measurement}";
        }
    }
}
