using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SongPanel : MonoBehaviour
{

    public TMP_Text songText;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void setSong(SongObject songName){
        songText.text = songName.song.name;
    }

}
