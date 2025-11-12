using System;
using System.Collections.Generic;
using Godot;

namespace GodotGOAPAI.Source.GOAP.Models;

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
}
