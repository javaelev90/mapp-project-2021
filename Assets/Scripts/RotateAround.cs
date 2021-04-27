using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{


    private Animator animator;

    public GameObject panel;

    private Quaternion panelRotation;

    private float movementDirection;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        animator.speed = 1;
        movementDirection = 0;
    }
    void Update()
    {
        panelRotation = panel.transform.rotation;

        if (Input.GetAxis("Mouse X") != 0)
        {
            movementDirection = Input.GetAxis("Mouse X");
        }

        if (Input.GetMouseButtonUp(0))
        {
            animator.speed = 1;
            if (movementDirection < 0)
            {
                //RotateLeft();
            }
            else if (movementDirection > 0)
            {
                //RotateRight();
            }

        }
    }

    public void RotateRight()
    {
        PlayClip("TurnAllRight", 360 - (panelRotation.eulerAngles.y / 360));
    }

    public void RotateLeft()
    {
        PlayClip("TurnAllLeft", panelRotation.eulerAngles.y / 360);

    }

    private void PlayClip(string clipName, float swipeTime)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                animator.Play(clip.name, 0, swipeTime);

                Invoke("SetBoolFalse", 0.5f);

                break;
            }
        }

    }

    private void SetBoolFalse()
    {
        animator.speed = 0;
    }

}
