namespace Loupedeck.HomeAssistant.Commands
{
    using System;

    using Newtonsoft.Json.Linq;

    public class LockCommand : DualStateCommand
    {
        // my son decided that the offState is locked :)
        public LockCommand() : base("Lock", offState: "locked", onState: "unlocked") { }

        protected override Boolean EntitiyFilter(String entity_id)
            => entity_id.StartsWith("lock.");

        protected override void RunCommand(String entity_id)
        {
            var service = this.GetCurrentState(entity_id).Name.Equals(this.OffState) ? "unlock" : "lock";
            var data = new JObject {
                { "domain", "lock" },
                { "service", service },
                { "target", new JObject { { "entity_id", entity_id } } }
            };

            this.GetPlugin().CallService(data);
        }
    }
}
