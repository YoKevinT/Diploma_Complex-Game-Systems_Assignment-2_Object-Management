using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistableObject
{
    static int colorPropertyId = Shader.PropertyToID("_Color");
    static MaterialPropertyBlock sharedPropertyBlock;

    [SerializeField] MeshRenderer[] meshRenderers;
    Color[] colors;

    List<ShapeBehavior> behaviorList = new List<ShapeBehavior>();

    void Awake()
    {
        colors = new Color[meshRenderers.Length];
    }

    public void GameUpdate()
    {
        // Increase the age by the time delta
        Age += Time.deltaTime;
        // Rotation and Movement
        for (int i = 0; i < behaviorList.Count; i++)
        {
            behaviorList[i].GameUpdate(this);
        }
    }

    public int ShapeId
    {
        get { return shapeId; }

        set
        {
            // int.MinValue = minimum integer
            if (shapeId == int.MinValue && value != int.MinValue)
            {
                shapeId = value;
            }
            else
            {
                Debug.LogError("Not allowed to change shapeId.");
            }
        }
    }

    int shapeId = int.MinValue;

    public int MaterialId { get; private set; }

    public void SetMaterial(Material material, int materialId)
    {
        // Loop through all renderers and set their materials
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = material;
        }

        MaterialId = materialId;
    }

    public void SetColor(Color color)
    {
        // This functions is so there is no material duplication
        if (sharedPropertyBlock == null)
        {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }
        sharedPropertyBlock.SetColor(colorPropertyId, color);

        // Loop through all renderers and set their colors
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            colors[i] = color;
            meshRenderers[i].SetPropertyBlock(sharedPropertyBlock);
        }
    }

    // Adjusts a single color element, identified via an index parameter
    public void SetColor(Color color, int index)
    {
        if (sharedPropertyBlock == null)
        {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }
        sharedPropertyBlock.SetColor(colorPropertyId, color);
        colors[index] = color;
        meshRenderers[index].SetPropertyBlock(sharedPropertyBlock);
    }

    public int ColorCount
    {
        get
        {
            return colors.Length;
        }
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(colors.Length);

        for (int i = 0; i < colors.Length; i++)
        {
            writer.Write(colors[i]);
        }

        writer.Write(Age);
        // First write its type then invoke its own save
        writer.Write(behaviorList.Count);

        for (int i = 0; i < behaviorList.Count; i++)
        {
            writer.Write((int)behaviorList[i].BehaviorType);
            behaviorList[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);

        // If the version is 5 or higher SetColor for each element
        if (reader.Version >= 5)
        {
            LoadColors(reader);
        }
        else
        {
            // If the version is 1 read color, otherwise use white
            SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
        }

        // Reads its identifier integer, cast it to ShapeBehaviorType, invoke AddBehavior and then load the rest behaviors
        if (reader.Version >= 6)
        {
            Age = reader.ReadFloat();
            int behaviorCount = reader.ReadInt();

            for (int i = 0; i < behaviorCount; i++)
            {
                ShapeBehavior behavior = ((ShapeBehaviorType)reader.ReadInt()).GetInstance();
                behaviorList.Add(behavior);
                behavior.Load(reader);
            }
        }
        // If is old use the old code    
        else if (reader.Version >= 4)
        {
            AddBehavior<RotationShapeBehavior>().AngularVelocity = reader.ReadVector3();
            AddBehavior<MovementShapeBehavior>().Velocity = reader.ReadVector3();
        }
    }

    void LoadColors(GameDataReader reader)
    {
        int count = reader.ReadInt();
        int max = count <= colors.Length ? count : colors.Length;
        int i = 0;

        for (; i < max; i++)
        {
            SetColor(reader.ReadColor(), i);
        }

        if (count > colors.Length)
        {
            for (; i < count; i++)
            {
                reader.ReadColor();
            }
        }

        else if (count < colors.Length)
        {
            for (; i < colors.Length; i++)
            {
                SetColor(Color.white, i);
            }
        }
    }

    public ShapeFactory OriginFactory
    {
        get
        {
            return originFactory;
        }
        set
        {
            if (originFactory == null)
            {
                originFactory = value;
            }
            else
            {
                Debug.LogError("Not allowed to change origin factory.");
            }
        }
    }

    ShapeFactory originFactory;

    public void Recycle()
    {
        Age = 0f;

        for (int i = 0; i < behaviorList.Count; i++)
        {
            behaviorList[i].Recycle();
        }

        behaviorList.Clear();
        OriginFactory.Reclaim(this);
    }

    public T AddBehavior<T>() where T : ShapeBehavior, new()
    {
        T behavior = ShapeBehaviorPool<T>.Get();
        behaviorList.Add(behavior);
        return behavior;
    }

    public float Age { get; private set; }
}