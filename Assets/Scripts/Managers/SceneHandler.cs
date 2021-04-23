using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This handler will keep the assigned scene persistent.
/// 
/// Remember to add "Managers" aka "the persistent scene" to Scenes in Build Settings.
/// </summary>
public class SceneHandler : MonoBehaviour
{
    [Tooltip("The scene you want to be persistent")]
    [SerializeField] Object persistentScene;

    void Awake()
    {
        if(!SceneManager.GetSceneByName(persistentScene.name).isLoaded)
            SceneManager.LoadSceneAsync(persistentScene.name, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Unloads current active scene and load
    /// </summary>
    /// <param name="sceneName"></param>
    public void ChangeScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName)); // funkar inte på samma frame...
    }
}
