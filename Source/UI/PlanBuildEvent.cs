using GodotGOAPAI.Source.EventSystem;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.UI;

public class PlanBuildEvent : IEvent
{
    public EntityType BuildingType { get; }
    
    public PlanBuildEvent(EntityType buildingType)
    {
        BuildingType = buildingType;
    }
}