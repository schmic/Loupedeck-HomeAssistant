namespace Loupedeck.HaPlugin.Commands
{
    using System;

    using Newtonsoft.Json.Linq;

    public class LightCommand : DualStateCommand
    {
        public LightCommand() : base("Light")
        { }

        protected override Boolean EntitiyFilter(String entity_id)
            => entity_id.StartsWith("light.");

        protected override void RunCommand(String entity_id)
        {
            var data = new JObject {
                { "domain", "light" },
                { "service", "toggle" },
                { "target", new JObject { { "entity_id", entity_id } } }
            };

            this.GetPlugin().CallService(data);
        }
    }
}
