namespace Loupedeck.HaPlugin.Commands
{
    using System;
    using Newtonsoft.Json.Linq;

    public class SwitchCommand : DualStateCommand
    {
        public SwitchCommand() : base("Switch")
        { }

        protected override Boolean EntitiyFilter(String entity_id)
            => entity_id.StartsWith("switch.");

        protected override void RunCommand(String entity_id)
        {
            var data = new JObject {
                { "domain", "switch" },
                { "service", "toggle" },
                { "target", new JObject { { "entity_id", entity_id } } }
            };

            this.GetPlugin().CallService(data);
        }
    }
}