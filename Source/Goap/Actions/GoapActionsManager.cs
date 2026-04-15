using System.Collections.Generic;
using System.Linq;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.Actions.Cut_Tree;

namespace GodotGOAPAI.Source.Goap.Actions;

public class GoapActionsManager : IGoapActionsManager
{
    private readonly List<IGoapAction> _goapActions = new List<IGoapAction>();
    
    // Use this like a factory, register all actions here once and then use them in the planner
    public void RegisterActions()
    {
        //_goapActionsAndResults.Add(new GoapActionCutTree());
    }
    
    public IReadOnlyList<IGoapAction> GetActions()
    {
        return _goapActions;
    }
}