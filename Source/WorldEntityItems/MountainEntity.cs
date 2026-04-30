using Godot;
using GodotGOAPAI.Source.WorldEntityItems.Abstractions;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.WorldEntityItems;

public partial class MountainEntity : ResourceInteractableEntityBase
{
	protected override string AnimationName => "bloop";
	public override EntityType EntityType => EntityType.Mountain;
	public override EntityType RequiredEntityTypeForInteraction => EntityType.Hammer;
	public override EntityType ResourceEntityTypeToSpawnOnDestroy => EntityType.Stone;
	public override int ResourceToSpawnAmount => 4;
	public override int Durability { get; protected set; } = 5;
}
