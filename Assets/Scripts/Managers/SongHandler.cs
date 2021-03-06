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

    public AudioClip GetSongBeatSFX()
    {
        return selectedSong != null ? selectedSong.sfx : null;
    }

    public List<float> GetAudioClipBeats()
    {
        return selectedSong != null ? selectedSong.GetBeats() : null;
    }

    public List<SpawnPatternChange> GetSpawnChangePatterns()
    {
        return selectedSong != null ? selectedSong.GetChangePatterns() : null;
    }

    public float GetPreferredMarginTimeBeforeBeat()
    {
        return selectedSong != null ? selectedSong.preferredMarginTimeBeforeBeat : 1f;
    }

    public float GetAudioLatency()
    {
        return Database.Instance.playerStatsRepository.GetLatency()/1000f;
    }

    public string GetUniqueSongName()
    {
        return selectedSong.uniqueName;
    }

    public string GetSongDifficulty()
    {
        return selectedSong.songDifficulty.ToString();
    }
}
