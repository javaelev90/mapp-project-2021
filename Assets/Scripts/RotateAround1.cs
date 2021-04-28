using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class RotateAround1 : MonoBehaviour
{
    private PlayerControls input;

    private Animator animator;

    public GameObject panel;

    private Quaternion panelRotation;

    private float movementDirection;
    Vector2 mouseDeltaMove;

    private void Awake()
    {
        input = new PlayerControls();
        input.Player.MouseDelta.performed += ctx => mouseDeltaMove = ctx.ReadValue<Vector2>();
    }
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        animator.speed = 1;
        movementDirection = 0;
    }
    void Update()
    {
        panelRotation = panel.transform.rotation;

        if (Mouse.current.leftButton.isPressed)
        {
            print("Mouse is moving in this X-axis: " + mouseDeltaMove.x);
            animator.speed = 1;
            if (mouseDeltaMove.x < 0)
            {
                //RotateLeft();
            }
            else if (mouseDeltaMove.x > 0)
            {
                //RotateRight();
            }

        }
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
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
