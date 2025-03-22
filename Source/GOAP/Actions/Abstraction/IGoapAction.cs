using System.Threading.Tasks;
using Godot;

namespace GodotGOAPAI.Source.GOAP.Actions.Abstraction;

public interface IGoapAction
{
    void Initialize(IGoapActionParams goapActionParams);
    int CalculateCost();
    bool IsActionPreconditionsValid();
    bool IsActionEffectsValid();
    void ExecuteAction(float deltaTime);
    bool IsCompletedConditionMet();
}