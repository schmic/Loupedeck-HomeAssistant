namespace Loupedeck.HaPlugin.Commands
{
    using System;
    using Newtonsoft.Json.Linq;

    public class ScriptCommand : BaseCommand
    {
        public ScriptCommand() : base("Script") { }

        protected override Boolean EntitiyFilter(String entity_id)
            => entity_id.StartsWith("script.");

        protected override void RunCommand(String entity_id)
        {
            var data = new JObject {
                { "domain", "script" },
                { "service", "turn_on" },
                { "target", new JObject { { "entity_id", entity_id } } }
            };

            this.GetPlugin().CallService(data);
        }

        protected override String GetCommandDisplayName(String entity_id, PluginImageSize imageSize)
        {
            if (entity_id.IsNullOrEmpty())
            { return base.GetCommandDisplayName(entity_id, imageSize); }

            var states = this.GetStates();

            var FriendlyName = states[entity_id].FriendlyName;

            return $"{FriendlyName}";
        }
    }
}
