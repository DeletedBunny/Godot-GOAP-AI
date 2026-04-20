using System;
using System.Collections.Generic;
using Godot;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;

public class GoapWorldStateModel<TNode> where TNode : Node
{
	private readonly object _worldResourcesLock = new object();
	
	public Dictionary<EntityType, List<TNode>> ResourcesAmountByType { get; private set; }
	public List<TNode> AgentsList { get; private set; }

	public GoapWorldStateModel(Dictionary<EntityType, List<TNode>> resourcesAmountByType)
	{
		ResourcesAmountByType = resourcesAmountByType;
	}

	public GoapWorldStateModel<TNode> WithEmptyInitialization()
	{
		foreach (var type in Enum.GetValues<EntityType>())
		{
			if(ResourcesAmountByType.ContainsKey(type))
				continue;
			
			AddItems(type, new List<TNode>());
		}
		
		return this;
	}
	
	public void AddItems(EntityType itemType, List<TNode> items)
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

	public void RemoveItems(EntityType itemType, List<TNode> items)
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

	public GoapWorldStateModel<TNode> GetCopy()
	{
		var resourcesAmountByType = new Dictionary<EntityType, List<TNode>>();
		foreach (var resourceType in ResourcesAmountByType)
		{
			resourcesAmountByType.Add(resourceType.Key, new List<TNode>(resourceType.Value));
		}
		var worldStateCopy = new GoapWorldStateModel<TNode>(resourcesAmountByType).WithEmptyInitialization();
		return worldStateCopy;
	}
}
