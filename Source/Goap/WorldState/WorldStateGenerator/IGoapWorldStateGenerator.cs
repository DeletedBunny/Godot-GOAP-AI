using Godot;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;

namespace GodotGOAPAI.Source.Goap.WorldState.WorldStateGenerator;

public interface IGoapWorldStateGenerator
{
    GoapWorldStateModel GenerateWorldStateModel(Node worldCollectionsRootNode, Node agentCollectionsRootNode);
}