using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton that will be saved in the current scene and NOT persist between scene changes.
/// </summary>
/// <typeparam name="T">The class to make an instance of</typeparam>
public class SingletonPattern<T> : MonoBehaviour where T : Component
{
    static T instance;

    public static T Instance 
    {
        get
        {
            if(instance == null)
            {
                // Create GameObject with Singleton component
                GameObject go = new GameObject();
                go.name = typeof(T).Name;
                go.hideFlags = HideFlags.HideAndDontSave; // Don't see in hierarchy or save to scene
                instance = go.AddComponent<T>();
            }
            return instance;
        }
    }

    // Set instance if singleton pattern class has been initialized as a gameobject component
    public static void SetInstanceIfNull(T existingInstance)
    {
        if(instance == null){
            instance = existingInstance;
        }
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}

/// <summary>
/// Singleton that will be saved in the additive scene "Managers" and persist between scene changes.
/// </summary>
/// <typeparam name="T">The class to make an instance of</typeparam>
public class SingletonPatternPersistent<T> : MonoBehaviour where T : Component, IInitializeAble
{
    static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // Making sure to add Persistent Singleton to the Managers scene
                Scene activeScene = SceneManager.GetActiveScene();
                bool sceneChanged = false;
                if (activeScene.name != "Managers")
                {
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName("Managers"));
                    sceneChanged = true;
                }

                // Create GameObject with Singleton component
                GameObject go = new GameObject();
                go.name = typeof(T).Name;
                go.hideFlags = HideFlags.HideAndDontSave; // Don't see in hierarchy or save to scene
                instance = go.AddComponent<T>();
                go.GetComponent<T>().Initialize();

                // Return to active scene
                if(sceneChanged)
                    SceneManager.SetActiveScene(activeScene);
            }
            return instance;
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            print("Destroying persistent singleton: " + instance.name);
            instance = null;
        }
    }

}
