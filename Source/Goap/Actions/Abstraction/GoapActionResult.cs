using System.Collections.Generic;

namespace GodotGOAPAI.Source.Goap.Actions.Abstraction;

public class GoapActionResult
{
    private readonly List<KeyValuePair<string, int>> _actionResults = new List<KeyValuePair<string, int>>();

    public void AddActionResult(string actionResultKey, int actionResultValue)
    {
        _actionResults.Add(new KeyValuePair<string, int>(actionResultKey, actionResultValue));
    }
    
    public IReadOnlyList<KeyValuePair<string, int>> GetActionResults()
    {
        return _actionResults.AsReadOnly();
    }
}