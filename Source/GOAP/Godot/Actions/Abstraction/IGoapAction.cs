namespace GodotGOAPAI.Source.GOAP.Godot.Actions.Abstraction;

public interface IGoapAction
{
    void Initialize(GoapActionParamsBase goapActionParams);
    int CalculateCost();
    bool IsActionPreconditionsValid();
    bool IsActionEffectsValid();
    void ExecuteAction(float deltaTime);
    bool IsCompletedConditionMet();
}