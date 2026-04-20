using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Godot;

namespace GodotGOAPAI.Source.EventSystem;

public partial class EventBus : Node, IEventBus
{
	public static EventBus Instance { get; private set; }
	
	private readonly ConcurrentDictionary<string, ImmutableList<Action<IEvent>>> _handlersDictionary = new();

	public override void _Ready()
	{
		Instance = this;
	}

	public void Subscribe<T>(Action<IEvent> handler)
	{
			var eventName = typeof(T).Name;
			_handlersDictionary.AddOrUpdate(eventName, ImmutableList.Create(handler), (_, actionsList) => actionsList.Add(handler));
	}

	public void Unsubscribe<T>(Action<IEvent> handler)
	{
			var eventName = typeof(T).Name;
			
			if (!_handlersDictionary.TryGetValue(eventName, out var actionsList))
				return;
			
			var newActionsList = actionsList.Remove(handler);

			if (newActionsList.IsEmpty)
			{
				_handlersDictionary.TryRemove(eventName, out _);
			}
			else
			{
				_handlersDictionary.TryUpdate(eventName, newActionsList, actionsList);
			}
	}

	public void SendEvent(IEvent @event)
	{
			var eventName = @event.GetType().Name;
			if (!_handlersDictionary.TryGetValue(eventName, out var handlersList))
				return;

			foreach (var handler in handlersList)
			{
				handler.Invoke(@event);
			}
	}
	
	public void SendEvent<T>() where T : IEvent
	{
		SendEvent(Activator.CreateInstance<T>());
	}
}
