using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotGOAPAI.Source.Goap.CustomResource;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.GodotHelpers;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.WorldState.WorldStateGenerator;

public class GoapWorldStateGenerator : IGoapWorldStateGenerator
{
    public GoapWorldStateModel GenerateWorldStateModel(Node worldCollectionsRootNode, Node agentCollectionsRootNode)
    {
        var resourcesAmountByType = new Dictionary<EntityType, List<Node3D>>();
        AddStaticWorldResourcesToDictionary(worldCollectionsRootNode, resourcesAmountByType);
        AddDynamicItemsToDictionary(worldCollectionsRootNode, resourcesAmountByType);
        var worldState = new GoapWorldStateModel();
        foreach (var (key, value) in resourcesAmountByType)
            worldState.AddItems(key, value);
        return worldState;
    }

    private static void AddStaticWorldResourcesToDictionary(Node worldCollectionsRootNode, Dictionary<EntityType, List<Node3D>> resourcesAmountByType)
    {
        foreach (var collection in worldCollectionsRootNode.GetChildren())
        {
            if (collection.GetMetaData(GoapResource.Name) is not GoapResource resourceType)
                continue;
            
            var nodes = collection.GetChildren().OfType<Node3D>().ToList();
            resourcesAmountByType.Add(resourceType.EntityType, nodes);
        }
    }

    private static void AddDynamicItemsToDictionary(Node worldCollectionsRootNode, Dictionary<EntityType, List<Node3D>> resourcesAmountByType)
    {
        var itemsNode = worldCollectionsRootNode.GetNode<Node>("Items");
        foreach (var type in Enum.GetValues<EntityType>())
        {
            if (IgnoredWorldResourceTypes.IgnoreList.Contains(type))
                continue;
            
            var resourcesByType = itemsNode.GetChildren().OfType<Node3D>().Where(node =>
            {
                if (node.GetMetaData(GoapResource.Name) is not GoapResource resourceType)
                    throw new Exception("GoapResourceType metadata not found on node - " + node.Name);

                return resourceType.EntityType == type;
            }).ToList();
            
            resourcesAmountByType.Add(type, resourcesByType);
        }
    }
}