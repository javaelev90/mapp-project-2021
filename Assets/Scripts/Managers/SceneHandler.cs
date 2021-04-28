using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This handler will keep the assigned "persistentScene" persistent through scene-transitions.
/// Remember to add "Managers" aka "the persistent scene" to Scenes in Build Settings.
/// </summary>
public class SceneHandler : SingletonPatternPersistent<SceneHandler>, IInitializeAble
{
    [Tooltip("The scene you want to be persistent through scene-transitions")]
    [SerializeField] Scene persistentScene;
    //[SerializeField] Object mainMenuScene;

    void Awake()
    {
        persistentScene = SceneManager.GetSceneByName("Managers");
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Initialize() 
    {
    }

    /// <summary>
    /// Unloads current active scene and loads the parameterized
    /// </summary>
    /// <param name="sceneName">Scene to load</param>
    public void ChangeScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (persistentScene == null || !scene.IsValid()) // NOTE(christian): Getting one strange callback where persistentScene is null... this will fix for now atleast
            return;

        if (scene.name != persistentScene.name)
            SceneManager.SetActiveScene(scene);
    }

    // Maybe let SceneHandler manage loading screen. But the game might not really need one
    //void ShowLoadingScreen() => loadingInterface.SetActive(true);

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
    //    Scene
    //    print("All loaded. Active scene: " + SceneManager.GetActiveScene().name);
    //}
}
