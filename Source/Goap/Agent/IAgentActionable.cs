using Godot;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.Goap.Agent;

public interface IAgentActionable
{
    Vector3 GlobalPosition { get; }
    void AddItemToHand(IPickupEntity item);
    void RemoveItemFromHand();
    void TransferItemToTarget(Node3D target);
    bool IsEntityTypeInHand(EntityType entityType);
    bool IsHoldingAnyItemInHand();
    void InteractOn(IInteractableEntity entity, double deltaTime);
    void MoveTo(Node3D target, double deltaTime);
    bool IsNearPosition(Node3D target);
}