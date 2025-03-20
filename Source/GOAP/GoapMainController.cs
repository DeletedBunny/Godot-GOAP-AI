using Godot;
using GodotGOAPAI.Source.EventBus;
using GodotGOAPAI.Source.GOAP.Godot;
using GodotGOAPAI.Source.GOAP.Godot.WorldStateGenerator;
using GodotGOAPAI.Source.GOAP.Models;
using GodotGOAPAI.Source.GOAP.WorldStateGenerator;
using GodotGOAPAI.Source.World;

namespace GodotGOAPAI.Source.GOAP;

public partial class GoapMainController : Node
{
    private readonly IGoapWorldStateGenerator _worldStateGenerator;
    
    [Export]
    private Node _worldDataCollectionsNode;
    
    public GoapWorldStateModel CurrentWorldStateModel { get; private set; }

    public GoapMainController()
    {
        _worldStateGenerator = new GodotGoapWorldStateGenerator();
    }
    
    public override void _Ready()
    {
        EventBus.EventBus.Instance.Subscribe<WorldReadyEvent>(OnWorldReady);
    }

    private void OnWorldReady(IEvent _)
    {
        var worldDataCollectionsGodotNode = new GodotNode(_worldDataCollectionsNode);
        CurrentWorldStateModel = _worldStateGenerator.GenerateWorldStateModel(worldDataCollectionsGodotNode);
    }

    public override void _ExitTree()
    {
        EventBus.EventBus.Instance.Unsubscribe<WorldReadyEvent>(OnWorldReady);
    }
}