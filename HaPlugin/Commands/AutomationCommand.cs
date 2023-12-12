namespace Loupedeck.HomeAssistant.Commands
{
    using System;
    using Newtonsoft.Json.Linq;

    public class AutomationCommand : BaseCommand
    {
        public AutomationCommand() : base("Automation") { }

        protected override Boolean EntitiyFilter(String entity_id)
            => entity_id.StartsWith("automation.");

        protected override void RunCommand(String entity_id)
        {
            var data = new JObject {
                { "domain", "automation" },
                { "service", "trigger" },
                { "target", new JObject { { "entity_id", entity_id } } }
            };

            this.GetPlugin().CallService(data);
        }
    }
}
