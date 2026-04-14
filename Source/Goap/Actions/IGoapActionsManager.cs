using System.Collections.Generic;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;

namespace GodotGOAPAI.Source.Goap.Actions;

public interface IGoapActionsManager
{
    void RegisterActions();
    IReadOnlyList<IGoapAction> GetActions();
}