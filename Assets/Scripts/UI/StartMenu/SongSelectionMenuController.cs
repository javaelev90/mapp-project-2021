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
         + "<size=18px><b>" + LocalisationSystem.GetLocalisedValue("SongLength") + ":</b> " + (int) SongHandler.Instance.GetSongAudioClip().length + " " + LocalisationSystem.GetLocalisedValue("Seconds") + " " + "</size>\n"
         + "<size=18px><b>" + LocalisationSystem.GetLocalisedValue("SongDifficulty") + ":</b> " + SongHandler.Instance.GetSongDifficulty() + "</size>"
         + "<br>"
         + "<br>"
         + "<size=26px><b>" + LocalisationSystem.GetLocalisedValue("Score") + "</b> " + (songScore == -1 ? 0 : songScore) + "</size>";
    }

}
