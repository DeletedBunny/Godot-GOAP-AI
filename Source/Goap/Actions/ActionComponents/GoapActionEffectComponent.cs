using System;
using System.Collections.Generic;
using System.Linq;

namespace GodotGOAPAI.Source.GOAP.Actions.ActionComponents;

public class GoapActionEffectComponent
{
    public List<KeyValuePair<string, int>> Effects { get; init; } = new();

    public bool ContainsEffect(string actionKey)
    {
        return Effects.Any(item => item.Key.Equals(actionKey));
    }

    public List<KeyValuePair<string, int>> GetMatchingEffects(params string[] actionKeys)
    {
        return Effects.Where(item => actionKeys.Contains(item.Key)).ToList();
    }
}