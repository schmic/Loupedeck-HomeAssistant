namespace Loupedeck.HaPlugin.Events
{
    using System;

    public class StateChangedEventArgs(String Entity_Id) : EventArgs
    {
        public String Entity_Id { get; } = Entity_Id;

        public static StateChangedEventArgs Create(String Entity_Id) => new(Entity_Id);
    }
}
