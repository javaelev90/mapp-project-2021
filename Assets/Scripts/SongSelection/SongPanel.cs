using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SongPanel : MonoBehaviour
{

    public TMP_Text songText;

    public SongObject songObject;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void setSong(SongObject songName){
        
        songObject = songName;
    
        songText.text = songName.song.name;


    }

    public void SetSongObject()
    {
        SongHandler.Instance.SetSongObject(songObject);
    }

}
