using System.Collections.Generic;
using System.Linq;

namespace GodotGOAPAI.Source.GOAP.Actions.ActionComponents;

public class GoapActionEffectComponent
{
    public List<KeyValuePair<string, int>> Effects { get; init; }

    public bool ContainsEffect(string actionResultKey)
    {
        return Effects.Any(item => item.Key == actionResultKey);
    }

    public List<KeyValuePair<string, int>> GetMatchingEffects(params string[] actionResultKeys)
    {
        return Effects.Where(item => actionResultKeys.Contains(item.Key)).ToList();
    }
}