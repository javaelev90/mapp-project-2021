using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenuController : MonoBehaviour
{
    [Header("Main button")]
    [SerializeField] Button inGameMenuButton;
    [Header("The menu gameobject")]
    [SerializeField] GameObject inGameMenu;
    [Header("Menu buttons")]
    [SerializeField] Button resumeButton;
    [SerializeField] Button restartButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button exitToMainButton;
    [SerializeField] Button exitApplicationButton;

    public static bool gameIsPaused = false;

    void Start()
    {
        InitializeUI();
    }

    void TogglePause()
    {
        gameIsPaused = !gameIsPaused;
        if (gameIsPaused)
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }
        else
        {
            Time.timeScale = 1;
            AudioListener.pause = false;
        }
    }

    void InitializeUI()
    {
        inGameMenuButton.gameObject.SetActive(true);
        inGameMenu.gameObject.SetActive(false);
    }

    void ChangeScene(string sceneName)
    {
        print("activescene: " + SceneManager.GetActiveScene().name);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }

    #region OnButtons
    public void OnInGameMenu()
    {
        inGameMenu.SetActive(!inGameMenu.activeSelf);
        TogglePause();
    }
    public void OnSettings()
    {

    }
    public void OnResume()
    {
        inGameMenu.SetActive(!inGameMenu.activeSelf);
        TogglePause();
    }
    public void OnRestart()
    {
        OnResume();
        GameManager.Instance.RestartGame();
    }
    public void OnExitToMain()
    {
        //OnResume();
        //ChangeScene("Start");
    }
    public void OnExitApplication()
    {
        Application.Quit();
    }
    #endregion
}
