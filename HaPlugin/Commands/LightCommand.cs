namespace Loupedeck.HomeAssistant.Commands
{
    using System;

    public class LightCommand : DualStateCommand
    {
        public LightCommand() : base("Light")
        { }

        protected override Boolean EntitiyFilter(String entity_id) => entity_id.StartsWith("light.");

        protected override void RunCommand(String entity_id) => this.GetPlugin().LightToggle(entity_id);
    }
}
