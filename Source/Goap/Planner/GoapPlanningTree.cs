namespace GodotGOAPAI.Source.Goap.Planner;

public class GoapPlanningTree
{
    public GoapPlanningLeaf Root { get; private set; }

    public GoapPlanningTree(GoapPlanningLeaf root)
    {
        Root = root;
    }
}