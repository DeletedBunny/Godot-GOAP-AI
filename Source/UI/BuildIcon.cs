using Godot;
using GodotGOAPAI.Source.EventSystem;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.UI;

public partial class BuildIcon : PanelContainer
{
	[Export]
	private EntityType BuildingType { get; set; }

	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.Pressed)
		{
			if ((mouseButtonEvent.ButtonMask & MouseButtonMask.Left) != 0)
			{
				EventBus.Instance.SendEvent(new IconPressedEvent(BuildingType));
				AcceptEvent();
			}
		}
	}
}
