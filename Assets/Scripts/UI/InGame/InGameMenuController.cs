using UnityEngine;
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

    void Awake()
    {
        //InitializeUI(); // fucks up the inGameMenu.SetActive(true).... Make sure to enable/disable beforehand in scenes
    }

    void InitializeUI()
    {
        inGameMenuButton.gameObject.SetActive(true);
        inGameMenu.gameObject.SetActive(false);
    }

    #region OnButtons
    public void OnInGameMenu()
    {
        if (GameManager.Instance.GameIsPaused)
        {
            OnResume();
        }
        else
        {
            inGameMenu.SetActive(true);
            GameManager.Instance.TogglePause(true);
        }
    }
    public void OnSettings()
    {

    }
    public void OnResume()
    {
        inGameMenu.SetActive(false);
        GameManager.Instance.TogglePause(false);
    }
    public void OnRestart()
    {
        OnResume();
        GameManager.Instance.RestartGame();
    }
    public void OnExitToMain(string sceneName)
    {
        OnResume();
        SceneHandler.Instance.ChangeScene(sceneName);
    }
    public void OnExitApplication()
    {
        Application.Quit();
    }
#endregion
}
