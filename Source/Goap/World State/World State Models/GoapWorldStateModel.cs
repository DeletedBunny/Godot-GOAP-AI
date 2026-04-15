using System;
using System.Collections.Generic;
using Godot;
using GodotGOAPAI.Source.GOAP.Models;

namespace GodotGOAPAI.Source.Goap.World_State.World_State_Models;

public class GoapWorldStateModel<TNode> where TNode : Node
{
	private readonly object _worldResourcesLock = new object();
	
	public Dictionary<GoapResourceType, List<TNode>> ResourcesAmountByType { get; private set; }
	public List<TNode> AgentsList { get; private set; }

	public GoapWorldStateModel(Dictionary<GoapResourceType, List<TNode>> resourcesAmountByType)
	{
		ResourcesAmountByType = resourcesAmountByType;
	}

	public GoapWorldStateModel<TNode> WithEmptyInitialization()
	{
		foreach (var type in Enum.GetValues<GoapResourceType>())
		{
			if(ResourcesAmountByType.ContainsKey(type))
				continue;
			
			AddItems(type, new List<TNode>());
		}
		
		return this;
	}
	
	public void AddItems(GoapResourceType type, List<TNode> items)
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

	public void RemoveItems(GoapResourceType type, List<TNode> items)
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

	public GoapWorldStateModel<TNode> GetCopy()
	{
		var resourcesAmountByType = new Dictionary<GoapResourceType, List<TNode>>();
		foreach (var resourceType in ResourcesAmountByType)
		{
			resourcesAmountByType.Add(resourceType.Key, new List<TNode>(resourceType.Value));
		}
		var worldStateCopy = new GoapWorldStateModel<TNode>(resourcesAmountByType).WithEmptyInitialization();
		return worldStateCopy;
	}
}
