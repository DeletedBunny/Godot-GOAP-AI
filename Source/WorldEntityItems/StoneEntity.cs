using GodotGOAPAI.Source.WorldEntityItems.Abstractions;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.WorldEntityItems;

public partial class StoneEntity : PickupEntityBase
{
	public override EntityType EntityType => EntityType.Stone;
}
