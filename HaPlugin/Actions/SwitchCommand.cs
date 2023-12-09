namespace Loupedeck.HomeAssistant
{
    using System;

    public class SwitchCommand : DualStateCommand
    {
        public SwitchCommand() : base("Switch")
        { }

        protected override void RunCommand(String entity_id) => this.plugin.SwitchToggle(entity_id);
    }
}