using System;
using System.Collections.Generic;
using Godot;

namespace GodotGOAPAI.Source.EventBus;

public partial class EventBus : Node, IEventBus
{
	public static EventBus Instance { get; private set; }
	
	private readonly object _eventLock = new object();
	private readonly Dictionary<string, List<Action<IEvent>>> _handlersDictionary = new Dictionary<string, List<Action<IEvent>>>();

	public override void _Ready()
	{
		Instance = this;
	}

	public void Subscribe<T>(Action<IEvent> handler)
	{
		lock (_eventLock)
		{
			var eventName = typeof(T).Name;
			if (_handlersDictionary.TryGetValue(eventName, out var actionList))
			{
				actionList.Add(handler);
			}
			else
			{
				_handlersDictionary.Add(eventName, new List<Action<IEvent>>() { handler });
			}
		}
	}

	public void Unsubscribe<T>(Action<IEvent> handler)
	{
		lock (_eventLock)
		{
			var eventName = typeof(T).Name;
			
			if (!_handlersDictionary.TryGetValue(eventName, out var actionsList))
				return;
			
			actionsList.Remove(handler);
			
			if(_handlersDictionary[eventName].Count == 0)
				_handlersDictionary.Remove(eventName);
		}
	}

	public void SendEvent(IEvent @event)
	{
		lock (_eventLock)
		{
			var eventName = @event.GetType().Name;
			if (!_handlersDictionary.TryGetValue(eventName, out var handlersList))
				return;

			foreach (var handler in handlersList)
			{
				handler.Invoke(@event);
			}
		}
	}
}
