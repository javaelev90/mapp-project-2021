using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongHandler : SingletonPatternPersistent<SongHandler>, IInitializeAble
{

    [SerializeField] private SongObject selectedSong;

    void Awake()
    {
        SongHandler.SetInstanceIfNull(this);
    }


    // This is used from GUI when choosing a song
    public void SetSongObject(SongObject song)
    {
        selectedSong = song;
    }

    public void Initialize(){}
    public AudioClip GetSongAudioClip()
    {
        return selectedSong != null ? selectedSong.song : null;
    }

    public List<float> GetAudioClipBeats()
    {
        return selectedSong != null ? selectedSong.beats : null;
    }

    public float GetPreferredMarginTimeBeforeBeat()
    {
        return selectedSong != null ? selectedSong.preferredMarginTimeBeforeBeat : 1f;
    }

    public float GetAudioLatency()
    {
        //TODO use database here: Database.playerStatsRepository.GetLatency()
        return Database.Instance.playerStatsRepository.GetLatency()/1000;
    }

    public string GetSongName()
    {
        return selectedSong.uniqueName;
    }
}
