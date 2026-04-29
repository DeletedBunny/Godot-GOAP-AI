using Godot;

namespace GodotGOAPAI.Source;

public static class MathHelper
{
    private static readonly Vector3 ReachedPositionThreshold = new(0.2f, 0.2f, 0.2f);
    public static readonly float FloatEpsilon = 0.001f;

    public static bool IsNearPosition(Vector3 source, Vector3 destination)
    {
        return (source - destination).Abs() <= ReachedPositionThreshold;
    }
    
    public static Vector3 MultiplyWithDouble(this Vector3 vector, double value)
    {
        var precisionLossValue = (float)value;
        return vector * precisionLossValue;
    }
}