using Godot;
using GodotGOAPAI.Source.EventSystem;
using GodotGOAPAI.Source.Goap.WorldState;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateEvents;
using GodotGOAPAI.Source.GodotHelpers;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.UI;

public partial class BuildManager : Node
{
	[Export] private Node3D _ghostModeObject;
	private bool _isPlacingMode;
	private EntityType _selectedEntityType;

	public override void _Ready()
	{
		_ghostModeObject.Visible = false;
		_isPlacingMode = false;
		_selectedEntityType = EntityType.None;
		EventBus.Instance.Subscribe<IconPressedEvent>(OnStartPlacingMode);
	}

	private void OnStartPlacingMode(IEvent eventData)
	{
		if (eventData is not IconPressedEvent iconPressedEvent)
			return;
		
		if (_isPlacingMode)
			CancelPlacement();
		
		_selectedEntityType = iconPressedEvent.EntityType;
		_ghostModeObject.Visible = true;
		_isPlacingMode = true;
	}

	public override void _Process(double delta)
	{
		if (!_isPlacingMode)
			return;

		UpdateGhostPosition();
	}

	private void UpdateGhostPosition()
	{
		var mousePosition = GetViewport().GetMousePosition();
		var camera = GetViewport().GetCamera3D();
		
		var rayOrigin = camera.ProjectRayOrigin(mousePosition);
		var rayNormal = camera.ProjectRayNormal(mousePosition);
		
		var distance = -rayOrigin.Y / rayNormal.Y;
		_ghostModeObject.GlobalPosition = rayOrigin + rayNormal * distance;
		_ghostModeObject.GlobalPosition += new Vector3(0f, 1.5f, 0f);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.Pressed)
		{
			if ((mouseButtonEvent.ButtonMask & MouseButtonMask.Left) != 0)
				ConfirmPlacement();
			if ((mouseButtonEvent.ButtonMask & MouseButtonMask.Right) != 0)
				CancelPlacement();
		}
	}
	
	private void ConfirmPlacement()
	{
		if (!_isPlacingMode)
			return;
		
		_ghostModeObject.Visible = false;
		_isPlacingMode = false;
		var buildingScene = SceneLoader.LoadScene(_selectedEntityType);
		var instanceList = buildingScene.InstanceSceneOnNode3D(GoapWorldStateService.Instance.WorldItemsCollectionNode, _ghostModeObject.GlobalPosition);
		EventBus.Instance.SendEvent(new WorldStateChangedEvent()
		{
			ChangedNodes = new()
			{
				{ EntityType.BuildingZone, instanceList }
			}, 
			IsRemoved = false
		});
		EventBus.Instance.SendEvent(new PlanBuildEvent(_selectedEntityType));
		_selectedEntityType = EntityType.None;
	}
	
	private void CancelPlacement()
	{
		if (!_isPlacingMode)
			return;
		
		_selectedEntityType = EntityType.None;
		_ghostModeObject.Visible = false;
		_isPlacingMode = false;
	}

	public override void _ExitTree()
	{
		EventBus.Instance.Unsubscribe<IconPressedEvent>(OnStartPlacingMode);
	}
}
