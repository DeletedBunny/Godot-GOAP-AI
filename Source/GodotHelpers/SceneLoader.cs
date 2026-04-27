using System.Collections.Generic;
using Godot;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.GodotHelpers;

public static class SceneLoader
{
    public static PackedScene LoadScene(EntityType entityType)
    {
        ResourceToPathLookup.EntityToPath.TryGetValue(entityType, out var pathToResource);
        return GD.Load<PackedScene>(pathToResource);
    }

    public static List<Node3D> InstanceSceneOnNode3D(
        this PackedScene sceneToInstance, 
        Node nodeToInstanceOn, 
        Vector3 globalPositionToInstanceAt,
        int amountToInstance = 1)
    {
        List<Node3D> instanceList = new();
        
        for (int i = 0; i < amountToInstance; i++)
        {
            var instance = sceneToInstance.Instantiate<Node3D>();
            nodeToInstanceOn.AddChild(instance);
            instance.GlobalPosition = globalPositionToInstanceAt + new Vector3(i, 0, 0);
            var rotationY = GD.Randf() * 360;
            instance.RotationDegrees = new Vector3(0, rotationY, 0);
            instanceList.Add(instance);
        }

        return instanceList;
    }
}