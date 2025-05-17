using Godot;
using GodotGOAPAI.Source.GOAP.Models;

namespace GodotGOAPAI.Source.GOAP;

public partial class GodotGoapWorldStateService : Node
{
    public static GodotGoapWorldStateService Instance { get; private set; }

    private readonly object _lock = new object();
    
    public GoapWorldStateModel<Node3D> CurrentWorldStateModel { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public GoapWorldStateModel<Node3D> GetCopyOfCurrentWorldStateModel()
    {
        var resourceCopy = 
        var resourceRef = CurrentWorldStateModel.ResourcesAmountByType.;
        var worldStateCopy = new GoapWorldStateModel<Node3D>();
    }
}