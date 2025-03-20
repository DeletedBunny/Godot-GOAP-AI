using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotGOAPAI.Source.GOAP.Abstraction;
using GodotGOAPAI.Source.GOAP.Godot.CustomResource;
using GodotGOAPAI.Source.GOAP.Models;
using GodotGOAPAI.Source.GOAP.WorldStateGenerator;

namespace GodotGOAPAI.Source.GOAP.Godot.WorldStateGenerator;

public class GodotGoapWorldStateGenerator : IGoapWorldStateGenerator
{
    public GoapWorldStateModel GenerateWorldStateModel(IGameObject worldCollectionsRootNode)
    {
        if (worldCollectionsRootNode is not GodotNode node)
            throw new Exception("The node is not a GodotNode");
        
        var resourcesAmountByType = new Dictionary<GoapResourceType, int>();
        AddStaticWorldResourcesToDictionary(node, resourcesAmountByType);
        AddDynamicItemsToDictionary(node, resourcesAmountByType);
        return new GoapWorldStateModel(resourcesAmountByType).WithEmptyInitialization();
    }

    private void AddStaticWorldResourcesToDictionary(GodotNode worldCollectionsRootNode, Dictionary<GoapResourceType, int> resourcesAmountByType)
    {
        foreach (var collection in worldCollectionsRootNode.Self.GetChildren())
        {
            var resourceType = collection.GetMeta("GoapResourceType").Obj as GodotGoapResource;
            
            if(resourceType == null)
                continue;
            
            resourcesAmountByType.Add(resourceType.GoapResourceType, collection.GetChildCount());
        }
    }

    private void AddDynamicItemsToDictionary(GodotNode worldCollectionsRootNode, Dictionary<GoapResourceType, int> resourcesAmountByType)
    {
        var itemsNode = worldCollectionsRootNode.Self.GetNode<Node>("Items");
        var dynamicItemsList = itemsNode.GetChildren();
        foreach (var type in Enum.GetValues<GoapResourceType>())
        {
            if (IgnoredWorldResourceTypes.IgnoreList.Contains(type))
                continue;

            var resourcesByType = dynamicItemsList.Where(r =>
            {
                var resourceType = r.GetMeta("GoapResourceType").Obj as GodotGoapResource;
                
                if(resourceType == null)
                    throw new Exception("The GoapResourceType metadata is not a GodotGoapResource on node - " + r.Name);
                
                return resourceType.GoapResourceType == type;
            }).ToList();
            
            resourcesAmountByType.Add(type, resourcesByType.Count);
        }
    }
}