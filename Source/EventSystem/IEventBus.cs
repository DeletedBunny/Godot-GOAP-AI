using System;

namespace GodotGOAPAI.Source.EventSystem;

public interface IEventBus
{
    void Subscribe<T>(Action<IEvent> handler);
    void Unsubscribe<T>(Action<IEvent> handler);
    void SendEvent(IEvent @event);
    void SendEvent<T>() where T : IEvent;
}