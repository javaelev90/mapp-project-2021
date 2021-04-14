using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Song Objects")]
public class SongObject : ScriptableObject
{

    public AudioClip song;
    public List<float> beats;

}
