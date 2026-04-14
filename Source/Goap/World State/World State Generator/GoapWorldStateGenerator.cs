using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotGOAPAI.Source.Goap.Custom_Resource;
using GodotGOAPAI.Source.GOAP.Models;

namespace GodotGOAPAI.Source.Goap.World_State.World_State_Generator;

public class GoapWorldStateGenerator<TNode> : IGoapWorldStateGenerator<TNode> where TNode : Node
{
    public GoapWorldStateModel<TNode> GenerateWorldStateModel(Node worldCollectionsRootNode, Node agentCollectionsRootNode)
    {
        var resourcesAmountByType = new Dictionary<GoapResourceType, List<TNode>>();
        AddStaticWorldResourcesToDictionary(worldCollectionsRootNode, resourcesAmountByType);
        AddDynamicItemsToDictionary(worldCollectionsRootNode, resourcesAmountByType);
        return new GoapWorldStateModel<TNode>(resourcesAmountByType).WithEmptyInitialization();
    }

    private static void AddStaticWorldResourcesToDictionary(Node worldCollectionsRootNode, Dictionary<GoapResourceType, List<TNode>> resourcesAmountByType)
    {
        foreach (var collection in worldCollectionsRootNode.GetChildren())
        {
            var resourceType = collection.GetMeta("GoapResourceType").Obj as GoapResource;
            
            if(resourceType == null)
                continue;
            
            var nodes = collection.GetChildren().OfType<TNode>().ToList();
            resourcesAmountByType.Add(resourceType.GoapResourceType, nodes);
        }
    }

    private static void AddDynamicItemsToDictionary(Node worldCollectionsRootNode, Dictionary<GoapResourceType, List<TNode>> resourcesAmountByType)
    {
        var itemsNode = worldCollectionsRootNode.GetNode<Node>("Items");
        foreach (var type in Enum.GetValues<GoapResourceType>())
        {
            if (IgnoredWorldResourceTypes.IgnoreList.Contains(type))
                continue;
            
            var resourcesByType = itemsNode.GetChildren().OfType<TNode>().Where(node =>
            {
                if (node.GetMeta("GoapResourceType").Obj is not GoapResource resourceType)
                    throw new Exception("GoapResourceType metadata not found on node - " + node.Name);

                return resourceType.GoapResourceType == type;
            }).ToList();
            
            resourcesAmountByType.Add(type, resourcesByType);
        }
    }
}