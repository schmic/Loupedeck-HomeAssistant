namespace Loupedeck.HomeAssistant
{
    using System;

    public class LightCommand : DualStateCommand
    {
        public LightCommand() : base("Light")
        { }

        protected override void RunCommand(String entity_id) => this.plugin.LightToggle(entity_id);
    }
}
