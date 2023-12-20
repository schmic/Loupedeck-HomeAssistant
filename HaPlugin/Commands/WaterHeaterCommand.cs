namespace Loupedeck.HomeAssistant.Commands
{
    using System;

    public class WaterHeaterCommand : BaseCommand
    {
        public WaterHeaterCommand() : base("Sensor") { }

        protected override Boolean EntitiyFilter(String entity_id)
            => entity_id.StartsWith("water_heater.");

        protected override void RunCommand(String entity_id) { }
    }
}
