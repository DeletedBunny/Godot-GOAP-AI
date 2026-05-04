using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.GOAP.Agent;
using GodotGOAPAI.Source.Goap.Planner;
using GodotGOAPAI.Source.Goap.WorldState;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.Goap.Agent;

public partial class Agent3D : Node3D, IAgentPlanner, IAgentActionable
{
    private const double MoveSpeed = 5;
    private readonly AgentInventory _agentHandsInventory = new(1);
    private readonly GoapPlannerExecutionQueue _planExecutionQueue = new();
    
    [Export]
    private Node3D _handPositionNode;

    private bool _isReadyToExecute;

    public bool IsReadyToPlan => _planExecutionQueue.IsQueueEmpty && !_isReadyToExecute;

    public override void _Process(double delta)
    {
        if (!_isReadyToExecute)
            return;
        
        if (_planExecutionQueue.IsQueueEmpty)
            _isReadyToExecute = false;

        try
        {
            _planExecutionQueue.ExecuteQueue(delta);
        }
        catch (Exception ex)
        {
            GD.PrintErr(ex, $"Agent had an error executing planned action queue. Action was: {ex.Data[GoapPlannerExecutionQueue.ExceptionActionKey]}");
        }
    }

    #region Agent Action Interface
    
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

    public void RemoveItemFromHand()
    {
        var item = _handPositionNode.GetChildren().FirstOrDefault();
        if (_agentHandsInventory.IsAnyItemInInventory 
            && item is IPickupEntity pickupEntity 
            && item is IEntity entity)
        {
            pickupEntity.Drop();
            _agentHandsInventory.RemoveItem(entity.EntityType);
            return;
        }
        
        GD.Print("Tried to remove item from hand that is not in hand");
    }

    public void TransferItemToTarget(Node3D target)
    {
        var item = _handPositionNode.GetChildren().FirstOrDefault();
        if (_agentHandsInventory.IsAnyItemInInventory
            && target is IDeliverResourceZone deliverResourceZone
            && item is IEntity entity
            && item is Node3D deliverableItem)
        {
            deliverResourceZone.DeliverResource(deliverableItem);
            _agentHandsInventory.RemoveItem(entity.EntityType);
            return;
        }
        
        GD.Print("Tried to transfer item to target that is not in hand");
    }
    
    public bool IsEntityTypeInHand(EntityType entityType) => _agentHandsInventory.HasItem(entityType);
    public bool IsHoldingAnyItemInHand() => _agentHandsInventory.IsAnyItemInInventory;

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
    
    #endregion

    #region Agent Planner Interface
    
    public List<(string, int)> GetAgentWorldState()
    {
        var inventoryState = _agentHandsInventory.GetInventoryState();
        if (inventoryState.Count == 0)
            inventoryState.Add((GoapWorldStateConstants.AgentHasEmptyHandsKey, 1));
        return inventoryState;
    }

    public void AddActionToExecutionQueue(IGoapAction action)
    {
        _planExecutionQueue.AddToQueue(action);
    }

    public void ExecuteActionQueue() => _isReadyToExecute = true;
    
    #endregion
}