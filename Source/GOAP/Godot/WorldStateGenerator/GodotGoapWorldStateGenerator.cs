using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotGOAPAI.Source.GOAP.Abstraction;
using GodotGOAPAI.Source.GOAP.Godot.CustomResource;
using GodotGOAPAI.Source.GOAP.Models;
using GodotGOAPAI.Source.GOAP.WorldStateGenerator;

namespace GodotGOAPAI.Source.GOAP.Godot.WorldStateGenerator;

public class GodotGoapWorldStateGenerator : IGoapWorldStateGenerator<Node, Node3D>
{
    public GoapWorldStateModel<Node3D> GenerateWorldStateModel(GameObject<Node> worldCollectionsRootNode)
    {
        if (worldCollectionsRootNode is not GameObject<Node> node)
            throw new Exception("The node is not a GodotNode");
        
        var resourcesAmountByType = new System.Collections.Generic.Dictionary<GoapResourceType, List<Node3D>>();
        AddStaticWorldResourcesToDictionary(node, resourcesAmountByType);
        AddDynamicItemsToDictionary(node, resourcesAmountByType);
        return new GoapWorldStateModel<Node3D>(resourcesAmountByType).WithEmptyInitialization();
    }

    private void AddStaticWorldResourcesToDictionary(GameObject<Node> worldCollectionsRootNode, System.Collections.Generic.Dictionary<GoapResourceType, List<Node3D>> resourcesAmountByType)
    {
        foreach (var collection in worldCollectionsRootNode.Self.GetChildren())
        {
            var resourceType = collection.GetMeta("GoapResourceType").Obj as GodotGoapResource;
            
            if(resourceType == null)
                continue;
            
            var nodes = collection.GetChildren().Where(node => node is Node3D).Cast<Node3D>().ToList();
            resourcesAmountByType.Add(resourceType.GoapResourceType, nodes);
        }
    }

    private void AddDynamicItemsToDictionary(GameObject<Node> worldCollectionsRootNode, System.Collections.Generic.Dictionary<GoapResourceType, List<Node3D>> resourcesAmountByType)
    {
        var itemsNode = worldCollectionsRootNode.Self.GetNode<Node>("Items");
        foreach (var type in Enum.GetValues<GoapResourceType>())
        {
            if (IgnoredWorldResourceTypes.IgnoreList.Contains(type))
                continue;

            var resourcesByType = itemsNode.GetChildren().Where(node =>
            {
                if(node is not Node3D)
                    return false;
                
                var resourceType = node.GetMeta("GoapResourceType").Obj as GodotGoapResource;
                
                if(resourceType == null)
                    throw new Exception("The GoapResourceType metadata is not a GodotGoapResource on node - " + node.Name);
                
                return resourceType.GoapResourceType == type;
            }).Cast<Node3D>().ToList();
            
            resourcesAmountByType.Add(type, resourcesByType);
        }
    }
}