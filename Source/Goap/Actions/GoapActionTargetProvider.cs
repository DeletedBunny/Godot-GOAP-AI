using Godot;
using GodotGOAPAI.Source.Goap.WorldState;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.Actions;

public class GoapActionTargetProvider
{
    public EntityType TargetType { get; set; }
    public Vector3 FromPosition { get; set; } = Vector3.Zero;
    public bool ShouldConsumeTarget { get; set; }
    
    public Node3D GetClosestTarget()
    {
        var worldStateService = GoapWorldStateService.Instance;
        if (ShouldConsumeTarget)
        {
            return worldStateService.GetClosestNodeFromWorldStateAndRemove(TargetType, FromPosition);
        }
        return worldStateService.GetClosestNodeFromWorldState(TargetType, FromPosition);
    }
}