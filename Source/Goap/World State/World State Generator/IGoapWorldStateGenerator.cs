using Godot;
using GodotGOAPAI.Source.GOAP.Models;

namespace GodotGOAPAI.Source.Goap.World_State.World_State_Generator;

public interface IGoapWorldStateGenerator<TNode> where TNode : Node
{
    GoapWorldStateModel<TNode> GenerateWorldStateModel(Node worldCollectionsRootNode, Node agentCollectionsRootNode);
}