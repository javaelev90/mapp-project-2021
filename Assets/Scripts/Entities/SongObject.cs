using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Song Objects")]
public class SongObject : ScriptableObject
{
    public string uniqueName;
    public AudioClip song;
    public AudioClip sfx;
    public float preferredMarginTimeBeforeBeat = 0.7f;
    [SerializeField] List<float> beats;

    [SerializeField] List<SpawnPatternChange> spawnPatternChanges;

    public void Initialize()
    {
        if(beats == null)
        {
            beats = new List<float>();
        }
        if(spawnPatternChanges == null){
            spawnPatternChanges = new List<SpawnPatternChange>();
        }
    }
    public List<float> GetBeats()
    {
        return beats;
    }

    public void AddBeat(float beat)
    {
        beats.Add(beat);
        ForceSerialization();
    }

    public void ClearBeats()
    {
        beats.Clear();
        ForceSerialization();
    }

    public List<SpawnPatternChange> GetChangePatterns()
    {
        return spawnPatternChanges;
    }

    public void AddChangePattern(SpawnPatternChange spawnPatternChange)
    {
        spawnPatternChanges.Add(spawnPatternChange);
        ForceSerialization();
    }

    public void ClearSpawnPatternChanges()
    {
        spawnPatternChanges.Clear();
        ForceSerialization();
    }
    private void ForceSerialization()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}

[System.Serializable]
public class SpawnPatternChange
{
    public SoulSpawner.SpawnPattern spawnPattern;
    public float activationTime;
}
