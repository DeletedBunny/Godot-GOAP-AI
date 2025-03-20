using Godot;

namespace GodotGOAPAI.Source.Camera;

public partial class CameraController : Node3D
{
	private const int MoveSpeed = 10;
	private const float FrictionForMovement = 0.2f;
	private const float RotateSpeed = 0.5f;
	private const float FrictionForRotation = 0.3f;
	private const int MinZoomLevel = 0;
	private const int MaxZoomLevel = 6;
	private const int ZoomStepSize = 3;
	
	private Camera3D _camera;
	private Vector3 _velocity = Vector3.Zero;
	
	private float _rotation;
	
	private int _zoomLevel = 1;
	private Vector3 _zoomActual;
	
	// Setup like this in editor (Y,Z) = (5,5) (Up,Back)
	private Vector3 _zoomDesired; 
	// Needed to simplify _zoomDesired to _zoomLevel * ZoomStepSize + _zoomOffset. Initial values will result in 5 and then +- steps of 3
	private float _zoomOffset; 

	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Camera3D");
		_zoomDesired = new Vector3(0, _camera.Position.Y, _camera.Position.Z);
		_zoomActual = _zoomDesired;
		// Can be Y or Z because we will only zoom forward and back with no rotation applied on camera
		_zoomOffset = _zoomDesired.Y - _zoomLevel * ZoomStepSize;
	}

	public override void _Process(double delta)
	{
		MoveCamera((float) delta);
		RotateCamera((float) delta);
		ZoomCamera((float) delta);
	}

	private void MoveCamera(float delta)
	{
		if (Input.IsActionPressed("camera_move_forward"))
		{
			_velocity -= Basis.Z - new Vector3(0, Basis.Z.Y, 0);
			_velocity = _velocity.Normalized();
		}

		if (Input.IsActionPressed("camera_move_backward"))
		{
			_velocity += Basis.Z - new Vector3(0, Basis.Z.Y, 0);
			_velocity = _velocity.Normalized();
		}

		if (Input.IsActionPressed("camera_move_left"))
		{
			_velocity -= Basis.X;
			_velocity = _velocity.Normalized();
		}

		if (Input.IsActionPressed("camera_move_right"))
		{
			_velocity += Basis.X;
			_velocity = _velocity.Normalized();
		}
		
		GlobalTranslate(_velocity * delta * MoveSpeed);
		_velocity = _velocity.Lerp(Vector3.Zero, FrictionForMovement);
	}

	private void RotateCamera(float delta)
	{
		if (Input.IsActionPressed("camera_rotate_left"))
			_rotation -= RotateSpeed;
		if (Input.IsActionPressed("camera_rotate_right"))
			_rotation += RotateSpeed;

		GlobalRotate(Vector3.Up, _rotation * delta);
		_rotation = Mathf.Lerp(_rotation, 0, FrictionForRotation);
	}

	private void ZoomCamera(float delta)
	{
		if (Input.IsActionJustReleased("camera_zoom_in"))
			_zoomLevel -= 1;
		if (Input.IsActionJustReleased("camera_zoom_out"))
			_zoomLevel += 1;
		
		_zoomLevel = Mathf.Clamp(_zoomLevel, MinZoomLevel, MaxZoomLevel);
		_zoomDesired.Y = _zoomLevel * ZoomStepSize + _zoomOffset;
		_zoomDesired.Z = _zoomLevel * ZoomStepSize + _zoomOffset;

		_zoomActual = _zoomActual.Lerp(_zoomDesired, 0.2f);
		
		if (!_zoomDesired.IsEqualApprox(_zoomActual))
			_camera.GlobalPosition = _zoomActual;
	}
}
