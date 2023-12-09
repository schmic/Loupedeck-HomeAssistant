namespace Loupedeck.HomeAssistant
{
    using System;
    using System.Collections.Generic;

    using Loupedeck.HomeAssistant.Events;

    abstract public class DualStateCommand : PluginMultistateDynamicCommand
    {
        protected HaPlugin plugin;
        protected readonly String type;

        public DualStateCommand(String type) : base()
        {
            this.type = type;
            this.AddState("off", "Off", $"{this.type} is off.");
            this.AddState("on", "On", $"{this.type} is on.");
        }

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
            PluginLog.Verbose($"{this.type}Command.OnLoad() => StatesReady");

            foreach (KeyValuePair<String, Json.HaState> group in this.plugin.States)
            {

                if (!group.Key.StartsWith($"{this.type.ToLower()}."))
                { continue; }

                var state = group.Value;
                this.AddParameter(state.Entity_Id, state.FriendlyName, this.type);
            }

            PluginLog.Info($"{this.type} => {this.GetParameters().Length} found.");
        }

        private void StateChanged(Object sender, StateChangedEventArgs e)
        {
            if (!e.Entity_Id.StartsWith($"{this.type.ToLower()}."))
            { return; }

            var entity_state = this.plugin.States[e.Entity_Id].State;
            var state_idx = entity_state.Equals("on") ? 1 : 0;

            this.SetCurrentState(e.Entity_Id, state_idx);
            this.ActionImageChanged(e.Entity_Id);
        }

        protected override BitmapImage GetCommandImage(String entity_id, Int32 stateIdx, PluginImageSize imageSize)
        {

            if (entity_id.IsNullOrEmpty())
            { return base.GetCommandImage(entity_id, stateIdx, imageSize); }

            var BgColor_on = new BitmapColor(73, 27, 74);
            var BgColor_off = new BitmapColor(22, 33, 38);
            var isOn = Convert.ToBoolean(stateIdx);

            var entity_friendly_name = this.plugin.States[entity_id].FriendlyName;
            var entity_img = isOn ?
                EmbeddedResources.ReadImage("Loupedeck.HomeAssistant.Resources.switch_on.png") :
                EmbeddedResources.ReadImage("Loupedeck.HomeAssistant.Resources.switch_off.png");

            var bitmapBuilder = new BitmapBuilder(imageSize);
            bitmapBuilder.FillRectangle(0, 0, bitmapBuilder.Width, bitmapBuilder.Height, isOn ? BgColor_on : BgColor_off);
            bitmapBuilder.DrawImage(entity_img, bitmapBuilder.Width / 2 - entity_img.Width / 2, 4);
            bitmapBuilder.DrawText(entity_friendly_name, 0, bitmapBuilder.Height / 4, bitmapBuilder.Width, bitmapBuilder.Height);
            return bitmapBuilder.ToImage();
        }

        protected override String GetCommandDisplayName(String entity_id, Int32 stateIdx, PluginImageSize imageSize)
        {
            if (entity_id.IsNullOrEmpty())
            { return ""; }

            var entityState = this.plugin.States[entity_id];
            return $"{entityState.FriendlyName}";
        }
    }
}
