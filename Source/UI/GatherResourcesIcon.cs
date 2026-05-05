using Godot;
using GodotGOAPAI.Source.EventSystem;

namespace GodotGOAPAI.Source.UI;

public partial class GatherResourcesIcon : PanelContainer
{
    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.Pressed)
        {
            if ((mouseButtonEvent.ButtonMask & MouseButtonMask.Left) != 0)
            {
                EventBus.Instance.SendEvent<GatherResourcesPressedEvent>();
                AcceptEvent();
            }
        }
    }
}