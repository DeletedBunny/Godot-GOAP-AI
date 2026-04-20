using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.WorldEntityItems.Interfaces;

public interface IResourceSpawningEntity
{
    EntityType ResourceEntityTypeToSpawnOnDestroy { get; }
    int ResourceToSpawnAmount { get; }
    int Durability { get; }
}