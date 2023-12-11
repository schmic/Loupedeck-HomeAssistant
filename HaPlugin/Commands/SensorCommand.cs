namespace Loupedeck.HomeAssistant.Commands
{
    using System;

    public class SensorCommand : BaseCommand
    {
        public SensorCommand() : base("Sensor") { }

        protected override Boolean EntitiyFilter(String entity_id)
            => entity_id.StartsWith($"sensor.") || entity_id.StartsWith($"binary_sensor.");
    }
}
