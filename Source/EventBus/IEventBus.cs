using System;

namespace GodotGOAPAI.Source.EventBus;

public interface IEventBus
{
    static EventBus Instance { get; private set; }
    void Subscribe<T>(Action<IEvent> handler);
    void Unsubscribe<T>(Action<IEvent> handler);
    void SendEvent(IEvent @event);
}