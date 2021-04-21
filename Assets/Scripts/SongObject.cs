using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Song Objects")]
public class SongObject : ScriptableObject
{
    public string uniqueName;
    public AudioClip song;
    public float preferredMarginTimeBeforeBeat = 0.7f;
    public List<float> beats;

}
