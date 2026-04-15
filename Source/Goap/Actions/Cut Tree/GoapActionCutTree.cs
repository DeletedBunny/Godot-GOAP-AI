using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.GOAP.Models;
using GodotGOAPAI.Source.Goap.World_State.World_State_Models;

namespace GodotGOAPAI.Source.Goap.Actions.Cut_Tree;

public class GoapActionCutTree : GoapActionBase
{
    protected override float DistanceCostMultiplier => 1;
    protected override float TimeCostMultiplier => 1;
    protected override float TimeCostInSeconds => 5;
    
    public override bool IsActionPreconditionsValid(Node3D agent, GoapWorldStateMemento<Node3D> worldStateMemento, GoapActionResult actionResult)
    {
        var isTreeInWorld = worldStateMemento.GetResource(GoapResourceType.Tree).Count > 0;
        var isAxeAsResult = actionResult.GetActionResults().FirstOrDefault(x => x.Key == "axe").Value > 0;

        if (isTreeInWorld && isAxeAsResult)
        {
            var closestNode = worldStateMemento.GetClosestElementByType(GoapResourceType.Tree, agent);
            worldStateMemento.AddModifiedResource(GoapResourceType.Tree, [closestNode], true);
        }
        
        return isTreeInWorld && isAxeAsResult;
    }

    public override GoapActionResult GetActionResult()
    {
        var actionResult = new GoapActionResult();
        actionResult.AddActionResult("log", 2);
        return actionResult;
    }
}