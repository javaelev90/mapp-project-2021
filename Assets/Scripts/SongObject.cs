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
    private void ForceSerialization()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
