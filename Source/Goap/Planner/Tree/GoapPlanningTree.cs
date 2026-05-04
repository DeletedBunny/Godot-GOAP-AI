using System.Collections.Generic;

namespace GodotGOAPAI.Source.Goap.Planner;

public class GoapPlanningTree
{
    public GoapPlanningLeaf Root { get; private set; }

    public GoapPlanningTree(GoapPlanningLeaf root)
    {
        Root = root;
    }

    public override string ToString()
    {
        string result = "########### Planning Tree Start ###########\n";
        Queue<GoapPlanningLeaf> queue = new();
        queue.Enqueue(Root);
        while (queue.Count > 0)
        {
            var currentLeaf = queue.Dequeue();
            result += currentLeaf;
            foreach (var leaves in currentLeaf.Children.Values)
            {
                leaves.ForEach(queue.Enqueue);
            }
        }

        return result;
    }
}