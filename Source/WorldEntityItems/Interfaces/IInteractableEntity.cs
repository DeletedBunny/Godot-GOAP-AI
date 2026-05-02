using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.WorldEntityItems.Interfaces;

public interface IInteractableEntity
{
    bool IsEntityInteractionFinished { get; }
    EntityType RequiredEntityTypeForInteraction { get; }
    void Interact(double deltaTime);
}