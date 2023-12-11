namespace Loupedeck.HomeAssistant.Commands
{
    using System;

    public class SwitchCommand : DualStateCommand
    {
        public SwitchCommand() : base("Switch")
        { }

        protected override Boolean EntitiyFilter(String entity_id) => entity_id.StartsWith("switch.");

        protected override void RunCommand(String entity_id) => this.GetPlugin().SwitchToggle(entity_id);
    }
}