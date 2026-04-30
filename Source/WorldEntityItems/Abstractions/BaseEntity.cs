using Godot;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.WorldEntityItems.Abstractions;

public abstract partial class BaseEntity : Node3D, IEntity
{
    public abstract EntityType EntityType { get; }
}