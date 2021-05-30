using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPageChangeController : MonoBehaviour
{

    [SerializeField] GameObject[] activatePanels;
    [SerializeField] GameObject[] deActivatePanels;
    [SerializeField] Animator menuChangeAnimator;
    [SerializeField] AnimationClip fadeIn;
    [SerializeField] AnimationClip fadeOut;

    public void ChangeMenuPage()
    {
        StartCoroutine(ChangePageRoutine());
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(ChangeSceneRoutine(sceneName));
    }

    public void FadeInScene()
    {
        menuChangeAnimator.Play(fadeIn.name);
    }

    IEnumerator ChangePageRoutine()
    {
        menuChangeAnimator.Play(fadeOut.name);
        yield return new WaitForSeconds(fadeOut.length);
        TogglePanels(false, deActivatePanels);
        TogglePanels(true, activatePanels);
        menuChangeAnimator.Play(fadeIn.name);
    }

    IEnumerator ChangeSceneRoutine(string sceneName)
    {
        menuChangeAnimator.Play(fadeOut.name);
        yield return new WaitForSeconds(fadeOut.length);
        if(sceneName != null) SceneHandler.Instance.ChangeScene(sceneName);
    }

    void TogglePanels(bool toggle, GameObject[] panels)
    {
        foreach(GameObject panel in panels)
        {
            panel.SetActive(toggle);
        }
    }

}
