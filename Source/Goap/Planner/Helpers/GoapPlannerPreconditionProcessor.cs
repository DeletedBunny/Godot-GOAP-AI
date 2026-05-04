using System.Collections.Generic;
using GodotGOAPAI.Source.Goap.WorldState;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;

namespace GodotGOAPAI.Source.GOAP.Planner.Helpers;

public static class GoapPlannerPreconditionProcessor
{
    public static int PreconditionOrdering(KeyValuePair<string, int> precondition)
    {
        if (precondition.Key.EndsWith(GoapWorldStateConstants.InBuildingZoneModifierPostfix))
            return 0;
        if (precondition.Key.Equals(GoapWorldStateConstants.AgentHasEmptyHandsKey))
            return 3;
        if (precondition.Key.StartsWith(GoapWorldStateConstants.HasModifierPrefix))
            return 1;
        return 2;
    }

    public static bool FilterPreconditions(KeyValuePair<string, int> precondition, GoapWorldStateModel simulationStateModel)
    {
        var stateValue = simulationStateModel.GetState(precondition.Key);
        return stateValue < precondition.Value;
    }
}