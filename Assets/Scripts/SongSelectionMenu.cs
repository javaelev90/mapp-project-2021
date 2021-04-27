using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SongSelectionMenu : MonoBehaviour
{
    public GameObject panel;
    public int mainMenu;

    public void StartGame() { 
        
    }

    public void GoBack()
    {
        if (panel.activeInHierarchy)
        {
            panel.SetActive(false);
        }
        else
        {
            SceneManager.LoadScene(mainMenu);
        }
    }

    public void SelectSong()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }

    }
}
