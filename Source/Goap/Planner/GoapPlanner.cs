using Godot;
using GodotGOAPAI.Source.Goap.Actions;
using GodotGOAPAI.Source.Goap.World_State;

namespace GodotGOAPAI.Source.Goap.Planner;

public class GoapPlanner
{
    [Export] private readonly IGoapWorldStateService _worldStateService = GoapWorldStateService.Instance;
    private readonly IGoapActionsManager _goapActionsManager = new GoapActionsManager();
    
    public GoapPlanner()
    {
        _goapActionsManager.RegisterActions();
    }

    public void Plan()
    {
    }
}