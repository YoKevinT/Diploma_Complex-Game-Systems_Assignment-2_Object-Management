using UnityEngine;

public abstract class ShapeBehavior
#if UNITY_EDITOR
    : ScriptableObject
#endif
{
#if UNITY_EDITOR
    public bool IsReclaimed { get; set; }

    public abstract void GameUpdate(Shape shape);

    public abstract void Save(GameDataWriter writer);
    public abstract void Load(GameDataReader reader);

    // Get a hold of the correct enumeration value
    public abstract ShapeBehaviorType BehaviorType { get; }

    // Recycle
    public abstract void Recycle();

    void OnEnable()
    {
        if (IsReclaimed)
        {
            Recycle();
        }
    }
#endif
}