using System;
using System.Collections.Generic;
using Godot;
using GodotGOAPAI.Source.GOAP.Abstraction;

namespace GodotGOAPAI.Source.GOAP.Models;

public class GoapWorldStateModel<T>
{
	private readonly object _worldResourcesLock = new object();
	
	public Dictionary<GoapResourceType, List<T>> ResourcesAmountByType { get; private set; }

	public GoapWorldStateModel(Dictionary<GoapResourceType, List<T>> resourcesAmountByType)
	{
		ResourcesAmountByType = resourcesAmountByType;
	}

	public GoapWorldStateModel<T> WithEmptyInitialization()
	{
		foreach (var type in Enum.GetValues<GoapResourceType>())
		{
			if(ResourcesAmountByType.ContainsKey(type))
				continue;
			
			AddItems(type, new List<T>());
		}
		
		return this;
	}
	
	public void AddItems(GoapResourceType type, List<T> items)
	{
		lock (_worldResourcesLock)
		{
			ResourcesAmountByType.TryGetValue(type, out var existingItems);
			if (existingItems == null)
			{
				ResourcesAmountByType.Add(type, items);
			}
			else
			{
				ResourcesAmountByType[type].AddRange(items);
			}
		}
	}

	public void RemoveItems(GoapResourceType type, List<T> items)
	{
		lock (_worldResourcesLock)
		{
			if (!ResourcesAmountByType.TryGetValue(type, out var existingItems))
				return;
			
			if (existingItems == null)
				return;
			
			ResourcesAmountByType[type].AddRange(items);
		}
	}
}
