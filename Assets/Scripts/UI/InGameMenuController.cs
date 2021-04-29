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


    SceneHandler sceneHandler;

    void Start()
    {
        InitializeUI();
        sceneHandler = SceneHandler.Instance;
    }

    void InitializeUI()
    {
        inGameMenuButton.gameObject.SetActive(true);
        inGameMenu.gameObject.SetActive(false);
    }

    void ChangeScene(string sceneName)
    {
        sceneHandler.ChangeScene(sceneName);
    }

    #region OnButtons
    public void OnInGameMenu()
    {
        inGameMenu.SetActive(!inGameMenu.activeSelf);
        GameManager.Instance.TogglePause(true);
    }
    public void OnSettings()
    {

    }
    public void OnResume()
    {
        inGameMenu.SetActive(!inGameMenu.activeSelf);
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
        sceneHandler.ChangeScene(sceneName);
    }
    public void OnExitApplication()
    {
        Application.Quit();
    }
    #endregion
}
