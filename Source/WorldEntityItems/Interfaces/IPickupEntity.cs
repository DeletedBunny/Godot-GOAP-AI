using Godot;

namespace GodotGOAPAI.Source.WorldEntityItems.Interfaces;

public interface IPickupEntity
{
    void Pickup(Node3D newOwner);
    void Drop();
}