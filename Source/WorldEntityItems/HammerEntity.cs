using GodotGOAPAI.Source.WorldEntityItems.Abstractions;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.WorldEntityItems;

public partial class HammerEntity : PickupEntityBase
{
	public override EntityType EntityType => EntityType.Hammer;
}
