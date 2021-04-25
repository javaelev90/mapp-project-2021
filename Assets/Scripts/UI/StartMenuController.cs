using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class StartMenuController : MonoBehaviour
{
    [Tooltip("The dropdown with songs")]
    [SerializeField] TMP_Dropdown songSelection;
    [Tooltip("The songs you have added to the dropdown, NOTE!: their position must match their positions in the dropdown")]
    [SerializeField] SongObject[] songObjects;

    [Header("Loading")]
    [SerializeField] GameObject loadingInterface;
    [SerializeField] Image loadingProgressBar;

    SongHandler songHandler;
    SceneHandler sceneHandler;

    //List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    void Awake()
    {
        // Load the Managers scen
        if (!SceneManager.GetSceneByName("Managers").isLoaded)
            SceneManager.LoadSceneAsync("Managers", LoadSceneMode.Additive);
    }
    void Start()
    {
        songHandler = SongHandler.Instance;
        songHandler.SetSongObject(songObjects[0]);

        sceneHandler = SceneHandler.Instance;
    }

    public void OnDropdownChange() => songHandler.SetSongObject(songObjects[songSelection.value]);

    public void OnStartGame(string sceneName)
    {
        sceneHandler.ChangeScene(sceneName);

        //HideMenu();
        //ShowLoadingScreen(true);
        //scenesToLoad.Add(SceneManager.LoadSceneAsync("Test1", LoadSceneMode.Additive));
        //StartCoroutine(LoadingScreen());
    }

    //void ShowLoadingScreen(bool show) => loadingInterface.SetActive(show);

    //IEnumerator LoadingScreen()
    //{
    //    float totalProgress = 0f;
    //    for (int i = 0; i < scenesToLoad.Count; i++)
    //    {
    //        while (!scenesToLoad[i].isDone)
    //        {
    //            totalProgress += scenesToLoad[i].progress;
    //            loadingProgressBar.fillAmount = totalProgress / scenesToLoad.Count;
    //            print("loading " + totalProgress);

    //            yield return null;
    //        }
    //    }
    //    ShowLoadingScreen(false);
    //}
}
