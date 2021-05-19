using UnityEngine;

public class PopupScreenController : MonoBehaviour
{
    [SerializeField] GameObject winScreenPanel;
    [SerializeField] GameObject looseScreenPanel;

    SceneHandler sceneHandler;

    void Start()
    {
        InitializeUI();
        sceneHandler = SceneHandler.Instance;
    }

    void InitializeUI()
    {
        winScreenPanel.gameObject.SetActive(false);
        looseScreenPanel.gameObject.SetActive(false);
    }

    public void OnRestart()
    {
        winScreenPanel.gameObject.SetActive(false);
        looseScreenPanel.gameObject.SetActive(false);
        GameManager.Instance.RestartGame();
    }
    public void OnExitToMain(string sceneName)
    {
        sceneHandler.ChangeScene(sceneName);
    }

    public void ToggleWinScreen(bool active)
    {
        winScreenPanel.gameObject.SetActive(active);
    }
    public void ToggleLooseScreen(bool active)
    {
        looseScreenPanel.gameObject.SetActive(active);
    }
}
