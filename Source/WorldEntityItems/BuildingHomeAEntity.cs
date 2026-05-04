using System;
using Godot;
using GodotGOAPAI.Source.EventSystem;
using GodotGOAPAI.Source.Goap.WorldState;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateEvents;
using GodotGOAPAI.Source.WorldEntityItems.Abstractions;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.WorldEntityItems;

public partial class BuildingHomeAEntity : BaseEntity, IInteractableEntity, IDeliverResourceZone
{
	[Export] private MeshInstance3D _mesh;
	[Export] private Node3D _resourceZone;
	[Export] private bool _isIcon = false;
	private ShaderMaterial _material;
	private double _deltaTimeCummulative;
	private bool _isBuildingDone;
	private float _buildProgress = 0.1f;
	private float _buildProgressMax = 1f;
	private float _resourceSpacing = 0.3f;
	private int _resourceRowAmount = 4;
	
	public override EntityType EntityType => EntityType.HomeA;
	public bool IsEntityInteractionFinished => _isBuildingDone;
	public EntityType RequiredEntityTypeForInteraction => EntityType.Hammer;
	
	public override void _Ready()
	{
		 _material = _mesh.GetActiveMaterial(0) as ShaderMaterial;
		 _material?.SetShaderParameter("clipping_height", _isIcon ? 1 : _buildProgress);
	}

	public void DeliverResource(Node3D resource)
	{
		if (_isIcon)
			return;
		
		var childrenAmount = _resourceZone.GetChildren().Count;
		var globalPosition = _resourceZone.GlobalPosition;
		var zPosition = globalPosition.Z + (_resourceSpacing * (childrenAmount % _resourceRowAmount));
		var xPosition = globalPosition.X + (_resourceSpacing * (childrenAmount / _resourceRowAmount));
		var positionToDropAt = new Vector3(xPosition, 1, zPosition);
		
		resource.GetParent().RemoveChild(resource);
		_resourceZone.AddChild(resource);
		resource.GlobalPosition = positionToDropAt;
	}
	
	public void Interact(double deltaTime)
	{
		if (_isIcon)
			return;
		
		if (IsEntityInteractionFinished)
			return;
		
		_deltaTimeCummulative += deltaTime;

		if (_deltaTimeCummulative < 1)
			return;
		
		_deltaTimeCummulative %= 1;

		if (_buildProgress < _buildProgressMax)
		{
			_buildProgress += 0.1f;
			_material?.SetShaderParameter("clipping_height", _buildProgress);
		}
		else if(!_isBuildingDone)
		{
			_material?.SetShaderParameter("clipping_height", 1);
			_isBuildingDone = true;
			foreach (var child in _resourceZone.GetChildren())
			{
				child.QueueFree();
			}
			EventBus.Instance.SendEvent(new WorldStateChangedEvent()
			{
				ChangedNodes = new()
				{
					{ EntityType.HomeA, [this] }
				}, 
				IsRemoved = false
			});
		}
	}
}
