using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SongSelectionMenuController : MonoBehaviour
{

    [SerializeField] SongObject songObject;
    [SerializeField] TMP_Text songInformationPanel;

    public void PopulateSongInformationPanel()
    {
        SongHandler.Instance.SetSongObject(songObject);
        int songScore = Database.Instance.songRepository.GetSongScore(songObject.uniqueName);
        songInformationPanel.text = songObject.song.name + "\n"
         + "<size=18px>Song length: " + songObject.song.length + " seconds " + "</size>\n"
         + "<size=20px>Score: " + (songScore == -1 ? 0 : songScore) + "</size>";
    }

    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
}
