namespace Loupedeck.HomeAssistant.Events
{
    using System;

    public class StateChangedEventArgs : EventArgs
    {
        public String Entity_Id { get; }

        public StateChangedEventArgs(String Entity_Id) => this.Entity_Id = Entity_Id;

        public static StateChangedEventArgs Create(String Entity_Id) => new StateChangedEventArgs(Entity_Id);
    }
}
