using System;
using System.Collections.Generic;

namespace GodotGOAPAI.Source.GOAP.Models;

public class GoapWorldStateModel
{
	private readonly object _worldResourcesLock = new object();
	
	public Dictionary<GoapResourceType, int> ResourcesAmountByType { get; private set; }

	public GoapWorldStateModel(Dictionary<GoapResourceType, int> resourcesAmountByType)
	{
		ResourcesAmountByType = resourcesAmountByType;
	}

	public GoapWorldStateModel WithEmptyInitialization()
	{
		foreach (var type in Enum.GetValues<GoapResourceType>())
		{
			if(ResourcesAmountByType.ContainsKey(type))
				continue;
			
			AddItems(type,0);
		}
		
		return this;
	}
	
	public void AddItems(GoapResourceType type, int amount)
	{
		lock (_worldResourcesLock)
		{
			ResourcesAmountByType.TryGetValue(type, out var itemsCount);
			if (itemsCount == 0)
			{
				ResourcesAmountByType.Add(type, amount);
			}
			else
			{
				ResourcesAmountByType[type] += amount;
			}
		}
	}

	public void RemoveItems(GoapResourceType type, int amount)
	{
		lock (_worldResourcesLock)
		{
			if (!ResourcesAmountByType.TryGetValue(type, out var itemsCount))
				return;
			
			if (itemsCount == 0)
				return;
			
			ResourcesAmountByType[type] -= amount;
		}
	}
}
