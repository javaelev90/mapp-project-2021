using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Song Objects")]
public class SongObject : ScriptableObject
{

    public AudioClip song;
    public float preferredMarginTimeBeforeBeat;
    public List<float> beats;

    public float audioDelay;

}
