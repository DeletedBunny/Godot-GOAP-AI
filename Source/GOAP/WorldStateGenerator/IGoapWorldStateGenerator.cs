using GodotGOAPAI.Source.GOAP.Models;

namespace GodotGOAPAI.Source.GOAP.WorldStateGenerator;

public interface IGoapWorldStateGenerator<T, TReturn>
{
    GoapWorldStateModel<TReturn> GenerateWorldStateModel(T worldCollectionsRootNode);
}