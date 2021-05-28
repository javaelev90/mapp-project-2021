using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{

    void Awake()
    {
        // Load the Managers scen first of all!
        if (!SceneManager.GetSceneByName("Managers").isLoaded)
            SceneManager.LoadSceneAsync("Managers", LoadSceneMode.Additive);
    }

    void Start()
    {
        // Limiting the FPS in start menu (or you get "over 9000" fps on desktop)
        if (SystemInfo.deviceType == DeviceType.Desktop)
            Application.targetFrameRate = 120;
        else if (Database.Instance.settingsRepository.GetFPSSetting() == 1f)
            Application.targetFrameRate = -1;
        else if (Database.Instance.settingsRepository.GetFPSSetting() == 0f)
            Application.targetFrameRate = Screen.currentResolution.refreshRate;

    }

}
