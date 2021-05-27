using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SongSelectionMenuController : MonoBehaviour
{

    [SerializeField] TMP_Text songInformationPanel;

    private void OnEnable() {
        PopulateSongInformationPanel();    
    }

    public void PopulateSongInformationPanel()
    {

        string uniqueName = SongHandler.Instance.GetUniqueSongName();

        int songScore = Database.Instance.songRepository.GetSongScore(uniqueName);
        songInformationPanel.text = SongHandler.Instance.GetSongAudioClip().name + "\n"
         + "<size=18px>Song length: " + SongHandler.Instance.GetSongAudioClip().length + " seconds " + "</size>\n"
         + "<size=20px>Score: " + (songScore == -1 ? 0 : songScore) + "</size>";
    }

}
