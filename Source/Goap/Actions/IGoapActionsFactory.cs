using System.Collections.Generic;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;

namespace GodotGOAPAI.Source.Goap.Actions;

public interface IGoapActionsFactory
{
    void RegisterActions();
    IGoapAction GetAction(GoapActionType type, GoapActionParams actionParams);
}