using System.Collections.Generic;
using System.Linq;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.GOAP.Actions.ActionComponents;

public class GoapActionPreconditionComponent
{
    public EntityType RequiredEntity { get; set; } = EntityType.None;
    public List<KeyValuePair<string, int>> Preconditions { get; set; } = [];
    
    public GoapActionPreconditionComponent Clone() => new()
    {
        RequiredEntity = RequiredEntity,
        Preconditions = Preconditions.ToList()
    };
}