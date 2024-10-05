namespace Loupedeck.HaPlugin.Commands
{
    using System;

    using Loupedeck.HaPlugin.Helpers;

    using Newtonsoft.Json.Linq;

    public class AutomationCommand : DualStateCommand
    {
        public AutomationCommand() : base("Automation") { }

        protected override Boolean EntitiyFilter(String entity_id)
            => entity_id.StartsWith("automation.");

        protected override void RunCommand(String entity_id) => this.CallService("trigger", entity_id);

        protected override Boolean ProcessTouchEvent(String entity_id, DeviceTouchEvent touchEvent)
        {
            if (touchEvent.IsLongPress())
            {
                PluginLog.Verbose($"ProcessTouchEvent.IsLongPress: {entity_id}");
                this.CallService("toggle", entity_id);
                return true;
            }

            return false;
        }

        protected override BitmapImage GetCommandImage(String entity_id, PluginImageSize imageSize)
        {
            if (entity_id.IsNullOrEmpty())
            { return base.GetCommandImage(entity_id, imageSize); }

            var states = this.GetStates();

            var isOn = states[entity_id].State.Equals("on");
            var entity_friendly_name = states[entity_id].FriendlyName;

            var colorRed = new BitmapColor(200, 10, 20);
            var colorGreen = new BitmapColor(10, 200, 20);

            var bitmapBuilder = new BitmapBuilder(imageSize);
            bitmapBuilder.FillCircle(0, 0, 20, isOn ? colorGreen : colorRed);
            bitmapBuilder.DrawText(entity_friendly_name, 0, 0, bitmapBuilder.Width, bitmapBuilder.Height);
            return bitmapBuilder.ToImage();
        }

        private void CallService(String service, String entity_id)
        {
            var data = new JObject {
                { "domain", "automation" },
                { "service", service },
                { "target", new JObject { { "entity_id", entity_id } } }
            };

            this.GetPlugin().CallService(data);
        }
    }
}
