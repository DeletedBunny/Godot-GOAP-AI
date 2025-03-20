using Godot;

namespace GodotGOAPAI.Source.World;

public partial class WorldLoaded : Node
{
	public override void _Ready()
	{
		// If used on root node this will be the last _Ready() called and therefore the entire scene tree is ready
		EventBus.EventBus.Instance.SendEvent(new WorldReadyEvent());
	}
}
