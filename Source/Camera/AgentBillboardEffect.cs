using Godot;

namespace GodotGOAPAI.Source.Camera;

[Tool]
public partial class AgentBillboardEffect : Node3D
{
    public override void _Process(double delta)
    {
        Camera3D camera;
        if (Engine.IsEditorHint())
        {
            camera = EditorInterface.Singleton.GetEditorViewport3D().GetCamera3D();
        }
        else
        {
            camera = GetViewport().GetCamera3D();
        }
        GlobalRotation = new Vector3(0, camera.GlobalRotation.Y , camera.GlobalRotation.Z);
    }
}