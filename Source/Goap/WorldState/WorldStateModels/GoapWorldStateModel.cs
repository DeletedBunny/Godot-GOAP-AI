using System;
using System.Collections.Generic;
using Godot;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;

public class GoapWorldStateModel
{
	private readonly object _worldResourcesLock = new object();
	
	public Dictionary<EntityType, List<Node3D>> ResourcesAmountByType { get; private set; }
	public List<Node3D> AgentsList { get; private set; }

	public GoapWorldStateModel(Dictionary<EntityType, List<Node3D>> resourcesAmountByType)
	{
		ResourcesAmountByType = resourcesAmountByType;
	}

	public GoapWorldStateModel WithEmptyInitialization()
	{
		foreach (var type in Enum.GetValues<EntityType>())
		{
			if(ResourcesAmountByType.ContainsKey(type))
				continue;
			
			AddItems(type, new List<Node3D>());
		}
		
		return this;
	}
	
	public void AddItems(EntityType itemType, List<Node3D> items)
	{
		lock (_worldResourcesLock)
		{
			ResourcesAmountByType.TryGetValue(itemType, out var existingItems);
			if (existingItems == null)
			{
				ResourcesAmountByType.Add(itemType, items);
			}
			else
			{
				ResourcesAmountByType[itemType].AddRange(items);
			}
		}
	}

	public void RemoveItems(EntityType itemType, List<Node3D> items)
	{
		lock (_worldResourcesLock)
		{
			if (!ResourcesAmountByType.TryGetValue(itemType, out var existingItems))
				return;
			
			if (existingItems == null)
				return;
			
			ResourcesAmountByType[itemType].AddRange(items);
		}
	}

	public GoapWorldStateModel GetCopy()
	{
		var resourcesAmountByType = new Dictionary<EntityType, List<Node3D>>();
		foreach (var resourceType in ResourcesAmountByType)
		{
			resourcesAmountByType.Add(resourceType.Key, new List<Node3D>(resourceType.Value));
		}
		var worldStateCopy = new GoapWorldStateModel(resourcesAmountByType).WithEmptyInitialization();
		return worldStateCopy;
	}
}
