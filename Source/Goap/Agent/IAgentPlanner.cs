using System.Collections.Generic;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;

namespace GodotGOAPAI.Source.Goap.Agent;

public interface IAgentPlanner
{
    bool IsReadyToPlan { get; }
    List<(string, int)> GetAgentWorldState();
    void AddActionToExecutionQueue(IGoapAction action);
    void ExecuteActionQueue();
}