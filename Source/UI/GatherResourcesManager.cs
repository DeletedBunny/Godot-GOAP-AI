using Godot;
using GodotGOAPAI.Source.EventSystem;
using GodotGOAPAI.Source.GodotHelpers;

namespace GodotGOAPAI.Source.UI;

public partial class GatherResourcesManager : Node
{
    private const string PathToPopup = "res://Resources/UI/GatherResourcesPopupUI.tscn";
    
    public override void _Ready()
    {
        EventBus.Instance.Subscribe<GatherResourcesPressedEvent>(OnGatherResourcesPressed);
    }

    private void OnGatherResourcesPressed(IEvent _)
    {
        var packedScene = GD.Load<PackedScene>(PathToPopup);
        var instance = packedScene.Instantiate<PanelContainer>();
        AddChild(instance);
    }

    public override void _ExitTree()
    {
        EventBus.Instance.Unsubscribe<GatherResourcesPressedEvent>(OnGatherResourcesPressed);
    }
}