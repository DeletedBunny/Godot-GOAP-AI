using Godot;
using GodotGOAPAI.Source.Goap.Planner;

namespace GodotGOAPAI.Source.Goap;

public partial class GoapMainController : Node
{
	private readonly GoapPlanner _planner = new GoapPlanner();
	
	private bool _start;

	public GoapMainController()
	{
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_up"))
			_start = true;
		
		//if (_start && _agentsCollectionNode.GetChildren().FirstOrDefault() is Node3D agent)
		{
			//var target = GoapGoapWorldStateService.Instance.GetClosestElementByType(GoapResourceType.Tree, agent);
			//if (target != null)
			{
				//_tempActionTest = new GoapMoveToAction();
				//_tempActionTest.Initialize(new GoapMoveToActionParams(target, agent, 3));
			}
		}
		
		//if (!(_tempActionTest?.IsCompletedConditionMet() ?? true))
			//_tempActionTest.ExecuteAction((float)delta);
	}
}
