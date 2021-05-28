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
         + "<size=18px>" + LocalisationSystem.GetLocalisedValue("SongLength") + ": " + SongHandler.Instance.GetSongAudioClip().length + " " + LocalisationSystem.GetLocalisedValue("Seconds") + " " + "</size>\n"
         + "<size=20px>" + LocalisationSystem.GetLocalisedValue("Score") + " " + (songScore == -1 ? 0 : songScore) + "</size>";
    }

}
