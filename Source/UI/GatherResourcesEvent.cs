using GodotGOAPAI.Source.EventSystem;

namespace GodotGOAPAI.Source.UI;

public class GatherResourcesEvent : IEvent
{
    public int LogAmount { get; }
    public int StoneAmount { get; }

    public GatherResourcesEvent(int logAmount, int stoneAmount)
    {
        LogAmount = logAmount;
        StoneAmount = stoneAmount;
    }
}