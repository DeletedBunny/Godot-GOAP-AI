using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.WorldEntityItems.Interfaces;

public interface IInteractableEntity
{
    bool IsEntityDestroyed { get; }
    EntityType RequiredEntityTypeForInteraction { get; }
    void Interact(double deltaTime);
}