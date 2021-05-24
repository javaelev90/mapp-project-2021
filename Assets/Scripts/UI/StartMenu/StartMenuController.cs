using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    [Tooltip("The dropdown with songs")]
    [SerializeField] TMP_Dropdown songSelection;
    [Tooltip("The songs you have added to the dropdown, NOTE!: their position must match their positions in the dropdown")]
    [SerializeField] SongObject[] songObjects;

    //[Header("Loading")]
    //[SerializeField] GameObject loadingInterface;
    //[SerializeField] Image loadingProgressBar;
    //List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    SongHandler songHandler;
    SceneHandler sceneHandler;


    void Awake()
    {
        // Load the Managers scen
        if (!SceneManager.GetSceneByName("Managers").isLoaded)
            SceneManager.LoadSceneAsync("Managers", LoadSceneMode.Additive);

        // Limiting the FPS in start menu
        if (SystemInfo.deviceType == DeviceType.Desktop)
            Application.targetFrameRate = 120;
        else if (Database.Instance.settingsRepository.GetFPSSetting() == 1f)
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
        else if (Database.Instance.settingsRepository.GetFPSSetting() == 0f)
            Application.targetFrameRate = -1;

        Debug.Log("Current screen refresh rate: " + Screen.currentResolution.refreshRate);
        Debug.Log("Current targetFrameRate: " + Application.targetFrameRate);
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

        // NOTE(christian): testar lite med loading screen
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
