using System;
using Godot;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.Goap.Agent;

public partial class Agent3D : Node3D
{
    private const double MoveSpeed = 5;
    private readonly AgentInventory _agentHandsInventory = new AgentInventory(1);
    
    [Export]
    private Node3D _handPositionNode;

    public void AddItemToHand(IPickupEntity item)
    {
        if (_agentHandsInventory.IsInventoryFull)
        {
            GD.Print("Agent inventory is full");
            return;
        }
        
        item.Pickup(_handPositionNode);
        
        if(item is IEntity entity)
            _agentHandsInventory.AddItem(entity.EntityType);
    }

    public void RemoveItemFromHand(IPickupEntity item)
    {
        if (item is IEntity entity
            && _agentHandsInventory.HasItem(entity.EntityType))
        {
            item.Drop();
            _agentHandsInventory.RemoveItem(entity.EntityType);
            return;
        }
        
        GD.Print("Tried to remove item from hand that is not in hand");
    }
    
    public bool HasEntityTypeInHand(EntityType entityType)
    {
        return _agentHandsInventory.HasItem(entityType);
    }

    public void InteractOn(IInteractableEntity entity, double deltaTime)
    {
        if (_agentHandsInventory.HasItem(entity.RequiredEntityTypeForInteraction))
        {
            entity.Interact(deltaTime);
        }
        else
        {
            throw new Exception(
                $"Tried to interact with entity that requires {entity.RequiredEntityTypeForInteraction} but agent does not have it in hand");
        }
    }

    public void MoveTo(Node3D target, double deltaTime)
    {
        var moveDirection = GlobalPosition.DirectionTo(target.GlobalPosition);
        GlobalTranslate(moveDirection.MultiplyWithDouble(MoveSpeed).MultiplyWithDouble(deltaTime));
    }

    public bool IsNearPosition(Node3D target)
    {
        return MathHelper.IsNearPosition(GlobalPosition, target.GlobalPosition);
    }
}