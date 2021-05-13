using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitAudioSource : MonoBehaviour
{
    [SerializeField] AudioSource audioSourceMusic;
    bool initializedAudio = false;
    void Update()
    {
        if(audioSourceMusic.isPlaying && !initializedAudio)
        {
            if(PlayerPrefs.HasKey("MusicStartTime"))
            {
                audioSourceMusic.time = PlayerPrefs.GetFloat("MusicStartTime");
                 
                for(int i = 0; i < SongHandler.Instance.GetAudioClipBeats().Count; i++)
                {
                    float beat = SongHandler.Instance.GetAudioClipBeats()[i];
                    if(beat > audioSourceMusic.time)
                    {
                        GameManager.Instance.SetBeatIndex(i);
                        break;
                    }
                }

            }
            initializedAudio = true;
            
        }
        
        if(PlayerPrefs.HasKey("RestartedGame"))
        {
            int restarted = PlayerPrefs.GetInt("RestartedGame");
            if(restarted == 1)
            {
                initializedAudio = false;
                PlayerPrefs.SetInt("RestartedGame", 0);
            }
        }
    }

}
