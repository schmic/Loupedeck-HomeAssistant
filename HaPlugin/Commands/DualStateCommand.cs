namespace Loupedeck.HaPlugin.Commands
{
    using System;
    using System.Collections.Generic;

    using Loupedeck.HaPlugin;
    using Loupedeck.HaPlugin.Events;
    using Loupedeck.HaPlugin.Helpers;

    public abstract class DualStateCommand : PluginMultistateDynamicCommand
    {
        protected readonly String OffState;
        protected readonly String OffLabel;

        protected readonly String OnState;
        protected readonly String OnLabel;

        public DualStateCommand(String groupName, String offState = "off", String onState = "on") : base()
        {
            this.GroupName = groupName;
            this.OffState = offState;
            this.OnState = onState;

            this.OffLabel = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.OffState.ToLower());
            this.OnLabel = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.OnState.ToLower());

            this.AddState(this.OffState, this.OffLabel, $"{this.GroupName} is {this.OffState}.");
            this.AddState(this.OnState, this.OnLabel, $"{this.GroupName} is {this.OnState}.");
        }
        protected abstract Boolean EntitiyFilter(String entity_id);

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

            PluginLog.Info($"[group: {this.GroupName}] [offLabel: {this.OffLabel}] [onLabel: {this.OnLabel}] [count: {this.GetParameters().Length}]");
        }

        private void StateChanged(Object sender, StateChangedEventArgs e)
        {
            if (!this.EntitiyFilter(e.Entity_Id))
            { return; }

            var states = this.GetStates();

            var entity_state = states[e.Entity_Id].State;
            var state_idx = entity_state.Equals(this.OnState) ? 1 : 0;

            this.SetCurrentState(e.Entity_Id, state_idx);
            this.ActionImageChanged(e.Entity_Id);
        }

        protected override BitmapImage GetCommandImage(String entity_id, PluginImageSize imageSize)
        {
            if (entity_id.IsNullOrEmpty())
            { return base.GetCommandImage(entity_id, imageSize); }

            var states = this.GetStates();

            var isOn = states[entity_id].State.Equals(this.OnState);
            var entity_friendly_name = states[entity_id].FriendlyName;

            var entity_img = isOn ?
                EmbeddedResources.ReadImage($"Loupedeck.HaPlugin.Resources.{this.GroupName.ToLower()}_{this.OnState}.png") :
                EmbeddedResources.ReadImage($"Loupedeck.HaPlugin.Resources.{this.GroupName.ToLower()}_{this.OffState}.png");

            var bitmapBuilder = new BitmapBuilder(imageSize);
            bitmapBuilder.DrawImage(entity_img, bitmapBuilder.Width / 2 - entity_img.Width / 2, 4);
            bitmapBuilder.DrawText(entity_friendly_name, 0, bitmapBuilder.Height / 4, bitmapBuilder.Width, bitmapBuilder.Height);
            return bitmapBuilder.ToImage();
        }

        protected override String GetCommandDisplayName(String entity_id, PluginImageSize imageSize)
        {
            if (entity_id.IsNullOrEmpty())
            { return ""; }

            var states = this.GetStates();

            var FriendlyName = states[entity_id].FriendlyName;
            return $"{FriendlyName}";
        }
    }
}
