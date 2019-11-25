public enum ShapeBehaviorType
{
    Movement,
    Rotation,
    Oscillation
}

public static class ShapeBehaviorTypeMethods
{
    // "this" is added to turn it into an extension method
    public static ShapeBehavior GetInstance(this ShapeBehaviorType type)
    {
        // When loading shape behavior, read and enumeration value and then add the correct behavior component
        switch (type)
        {
            case ShapeBehaviorType.Movement:
                return ShapeBehaviorPool<MovementShapeBehavior>.Get();
            case ShapeBehaviorType.Rotation:
                return ShapeBehaviorPool<RotationShapeBehavior>.Get();
            case ShapeBehaviorType.Oscillation:
                return ShapeBehaviorPool<OscillationShapeBehavior>.Get();
        }
        UnityEngine.Debug.Log("Forgot to support " + type);
        return null;
    }
}