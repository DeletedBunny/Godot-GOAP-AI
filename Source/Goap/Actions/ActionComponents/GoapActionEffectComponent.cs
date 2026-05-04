using System.Collections.Generic;
using System.Linq;

namespace GodotGOAPAI.Source.GOAP.Actions.ActionComponents;

public class GoapActionEffectComponent : IGoapActionEffectComponent
{
    public List<KeyValuePair<string, int>> Effects { get; private init; } = [];

    public bool ContainsEffect(string actionKey)
    {
        return Effects.Any(item => item.Key.Equals(actionKey));
    }

    public GoapActionEffectComponent Clone() => new()
    {
        Effects = Effects.ToList()
    };
}