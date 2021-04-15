using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongHandler : MonoBehaviour
{

    [SerializeField] private SongObject selectedSong;
    private static SongHandler _instance;
    public static SongHandler Instance
    {
        get
        {
            return _instance;
        }

    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            GameObject.Destroy(Instance);
            return;
        } else
        {
            _instance = this;
        }
        DontDestroyOnLoad(this);
    }

    // This is used from GUI when choosing a song
    public void SetSongObject(SongObject song)
    {
        selectedSong = song;
    }

    public AudioClip GetSongAudioClip()
    {
        return selectedSong != null ? selectedSong.song : null;
    }

    public List<float> GetAudioClipBeats()
    {
        return selectedSong != null ? selectedSong.beats : null;
    }

    public float GetPrefferdMarginTimeBeforeBeat()
    {
        return selectedSong != null ? selectedSong.prefferedMarginTimeBeforeBeat : 1f;
    }
}
