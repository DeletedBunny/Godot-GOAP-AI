using Godot;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;

namespace GodotGOAPAI.Source.Goap.WorldState.WorldStateGenerator;

public interface IGoapWorldStateGenerator<TNode> where TNode : Node
{
    GoapWorldStateModel<TNode> GenerateWorldStateModel(Node worldCollectionsRootNode, Node agentCollectionsRootNode);
}