using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotGOAPAI.Source.Goap.WorldState;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.Agent;

public class AgentInventory
{
    private readonly int _slots;
    private readonly Dictionary<EntityType, int> _inventory;
    
    public bool IsInventoryFull => _inventory?.Count >= _slots;
    public bool IsAnyItemInInventory => _inventory?.Count > 0;

    public AgentInventory(int slots, Dictionary<EntityType, int> startingInventory = null)
    {
        _slots = slots;
        if (startingInventory?.Count > slots)
        {
            _inventory = new Dictionary<EntityType, int>(startingInventory);
        }
        else if (startingInventory?.Count > 0)
        {
            _inventory = new Dictionary<EntityType, int>(slots);
            foreach (var (key, value) in startingInventory)
            {
                _inventory.Add(key, value);
            }
        }
        else
        {
            _inventory = new Dictionary<EntityType, int>(slots);
        }
    }

    public void AddItem(EntityType entityType)
    {
        var isResourceTypeValid = AgentInventoryHelper.InventoryConstants.TryGetValue(entityType, out var inventoryCapacity);
        var isItemAlreadyInInventory = _inventory.TryGetValue(entityType, out var amountInInventory);

        if (!isResourceTypeValid)
        {
            GD.Print("Tried to add invalid resource type to inventory of type " + entityType);
            return;
        }

        if (isItemAlreadyInInventory && amountInInventory < inventoryCapacity)
        {
            _inventory[entityType]++;
        }
        else if (!isItemAlreadyInInventory)
        {
            _inventory.Add(entityType, 1);
        }
        else
        {
            GD.Print("Tried to add more items than the inventory capacity of " + entityType);
        }
    }

    public bool RemoveItem(EntityType entityType)
    {
        var isItemInInventory = _inventory.TryGetValue(entityType, out var amountInInventory);

        if (!isItemInInventory)
            return false;
        
        if (amountInInventory > 1)
        {
            _inventory[entityType]--;
        }
        else
        {
            _inventory.Remove(entityType);
        }

        return true;
    }

    public bool HasItem(EntityType entityType, int requiredAmount = 1)
    {
        return _inventory.ContainsKey(entityType) && _inventory[entityType] >= requiredAmount;
    }
    
    public List<EntityType> GetEntitiesInInventory() => _inventory.Keys.ToList();

    public List<(string, int)> GetInventoryState()
    {
        return _inventory.Select(item => (GoapWorldStateConstants.HasModifierPrefix + item.Key, item.Value)).ToList();
    }
}