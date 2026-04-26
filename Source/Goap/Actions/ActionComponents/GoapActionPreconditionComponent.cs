using System.Collections.Generic;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.GOAP.Actions.ActionComponents;

public class GoapActionPreconditionComponent
{
    public EntityType RequiredEntity { get; set; } = EntityType.None;
    public List<KeyValuePair<string, int>> Preconditions { get; init; } = new();
    
    public bool NeedsEntityNearby()
    {
        return RequiredEntity != EntityType.None;
    }
}