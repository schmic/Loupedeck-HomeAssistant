namespace Loupedeck.HomeAssistant.Commands
{
    using System;
    using Newtonsoft.Json.Linq;

    public class SceneCommand : BaseCommand
    {
        public SceneCommand() : base("Scene") { }

        protected override Boolean EntitiyFilter(String entity_id)
            => entity_id.StartsWith("scene.");

        protected override void RunCommand(String entity_id)
        {
            var data = new JObject {
                { "domain", "scene" },
                { "service", "turn_on" },
                { "target", new JObject { { "entity_id", entity_id } } }
            };

            this.GetPlugin().CallService(data);
        }
    }
}
