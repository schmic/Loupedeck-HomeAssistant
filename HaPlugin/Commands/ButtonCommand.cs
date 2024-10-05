namespace Loupedeck.HaPlugin.Commands
{
    using System;
    using Newtonsoft.Json.Linq;

    public class ButtonCommand : BaseCommand
    {
        public ButtonCommand() : base("Button") { }

        protected override Boolean EntitiyFilter(String entity_id)
            => entity_id.StartsWith("button.");

        protected override void RunCommand(String entity_id)
        {
            var data = new JObject {
                { "domain", "button" },
                { "service", "press" },
                { "target", new JObject { { "entity_id", entity_id } } }
            };

            this.GetPlugin().CallService(data);
        }
    }
}
