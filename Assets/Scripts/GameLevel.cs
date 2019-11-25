using UnityEngine;

public class GameLevel : PersistableObject
{
    [SerializeField] SpawnZone spawnZone;
    [SerializeField] PersistableObject[] persistentObjects;

    public static GameLevel Current { get; private set; }

    public Shape SpawnShape()
    {
        return spawnZone.SpawnShape();
    }

    void OnEnable()
    {
        Current = this;
        if (persistentObjects == null)
        {
            persistentObjects = new PersistableObject[0];
        }
    }

    public override void Save(GameDataWriter writer)
    {
        // Save how may such objects there are, then save each of them
        writer.Write(persistentObjects.Length);
        for (int i = 0; i < persistentObjects.Length; i++)
        {
            persistentObjects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        // No need to instantiate because the level object are part of the scene
        int savedCount = reader.ReadInt();
        for (int i = 0; i < savedCount; i++)
        {
            persistentObjects[i].Load(reader);
        }
    }
}