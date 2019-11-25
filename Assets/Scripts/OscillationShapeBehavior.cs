﻿using UnityEngine;

public sealed class OscillationShapeBehavior : ShapeBehavior
{
    float previousOscillation;

    public override ShapeBehaviorType BehaviorType
    {
        get
        {
            return ShapeBehaviorType.Oscillation;
        }
    }

    public override void GameUpdate(Shape shape)
    {
        // The oscillation curve is simply the sine of 2π multiplied by the frequency and current time
        float oscillation = Mathf.Sin(2f * Mathf.PI * Frequency * shape.Age);
        shape.transform.localPosition += (oscillation - previousOscillation) * Offset;
        previousOscillation = oscillation;
    }
    public override void Save(GameDataWriter writer) 
    {
        writer.Write(Offset);
        writer.Write(Frequency);
        writer.Write(previousOscillation);
    }
    public override void Load(GameDataReader reader)
    {
        Offset = reader.ReadVector3();
        Frequency = reader.ReadFloat();
        previousOscillation = reader.ReadFloat();
    }

    public override void Recycle()
    {
        previousOscillation = 0f;
        ShapeBehaviorPool<OscillationShapeBehavior>.Reclaim(this);
    }

    public Vector3 Offset { get; set; }

    public float Frequency { get; set; }
}