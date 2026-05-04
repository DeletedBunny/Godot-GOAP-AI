using System.Collections.Generic;

namespace GodotGOAPAI.Source.GOAP.Actions.ActionComponents;

public interface IGoapActionEffectComponent
{
    List<KeyValuePair<string, int>> Effects { get; }
    bool ContainsEffect(string actionKey);
}