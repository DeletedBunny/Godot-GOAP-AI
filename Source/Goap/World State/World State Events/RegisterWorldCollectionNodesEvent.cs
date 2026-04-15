using Godot;
using GodotGOAPAI.Source.Event_System;

namespace GodotGOAPAI.Source.Goap.World_State.World_State_Events;

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