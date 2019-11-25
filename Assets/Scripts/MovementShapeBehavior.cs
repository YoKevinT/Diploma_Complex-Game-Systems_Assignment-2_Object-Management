using UnityEngine;

public sealed  class MovementShapeBehavior : ShapeBehavior
{
    public Vector3 Velocity { get; set; }

    public override void GameUpdate(Shape shape)
    {
        shape.transform.localPosition += Velocity * Time.deltaTime;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(Velocity);
    }

    public override void Load(GameDataReader reader)
    {
        Velocity = reader.ReadVector3();
    }

    public override ShapeBehaviorType BehaviorType
    {
        get
        {
            // Return to Movement
            return ShapeBehaviorType.Movement;
        }
    }

    public override void Recycle()
    {
        ShapeBehaviorPool<MovementShapeBehavior>.Reclaim(this);
    }
}