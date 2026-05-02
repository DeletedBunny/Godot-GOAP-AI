using GodotGOAPAI.Source.EventSystem;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.UI;

public class IconPressedEvent : IEvent
{
    public EntityType EntityType { get; }
    
    public IconPressedEvent(EntityType entityType)
    {
        EntityType = entityType;
    }
}