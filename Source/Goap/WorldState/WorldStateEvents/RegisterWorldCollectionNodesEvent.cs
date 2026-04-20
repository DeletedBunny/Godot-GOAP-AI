using Godot;
using GodotGOAPAI.Source.EventSystem;

namespace GodotGOAPAI.Source.Goap.WorldState.WorldStateEvents;

public class RegisterWorldCollectionNodesEvent : IEvent
{
    public Node WorldCollectionsRootNode { get; }
    public Node AgentCollectionsRootNode { get; }
    
    public RegisterWorldCollectionNodesEvent(Node worldCollectionsRootNode, Node agentCollectionsRootNode)
    {
        WorldCollectionsRootNode = worldCollectionsRootNode;
        AgentCollectionsRootNode = agentCollectionsRootNode;
    }
}