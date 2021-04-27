using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreenController : MonoBehaviour
{

    [SerializeField] GameObject winScreenPanel;

    SceneHandler sceneHandler;

    void Start()
    {
        InitializeUI();
        sceneHandler = SceneHandler.Instance;
    }

    void InitializeUI()
    {
        winScreenPanel.gameObject.SetActive(false);
    }

    public void OnRestart()
    {
        winScreenPanel.gameObject.SetActive(false);
        GameManager.Instance.RestartGame();
    }
    public void OnExitToMain(string sceneName)
    {
        sceneHandler.ChangeScene(sceneName);
    }
}
