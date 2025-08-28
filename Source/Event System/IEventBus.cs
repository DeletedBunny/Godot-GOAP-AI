using System;

namespace GodotGOAPAI.Source.Event_System;

public interface IEventBus
{
    static Event_System.EventBus Instance { get; private set; }
    void Subscribe<T>(Action<IEvent> handler);
    void Unsubscribe<T>(Action<IEvent> handler);
    void SendEvent(IEvent @event);
}