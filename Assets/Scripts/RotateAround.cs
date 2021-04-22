using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{


    private Animator animator;
    public GameObject panel;
    private Quaternion panelRotation;

    private float startTime;
    private float endTime;
    private float swipeTime;

    private float setAnimationStartTime;

    private Vector3 cylinderScreenPoint;
    private Vector3 startPosition;

    private bool isTurningRight = false;
    private bool isTurningLeft = false;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        animator.speed = 1;
        cylinderScreenPoint = Camera.main.WorldToScreenPoint(transform.position);
    }
    void Update()
    {
        panelRotation = panel.transform.rotation;

        animator.SetBool("IsTurningRight", isTurningRight);
        animator.SetBool("IsTurningLeft", isTurningLeft);

        if (Input.GetMouseButtonDown(0))
        {
            startTime = Time.time;
            startPosition = Input.mousePosition;

            if (startPosition.x < cylinderScreenPoint.x)
            {
                print("I'm left");
                RotateLeft();

            }
            else if (startPosition.x > cylinderScreenPoint.x)
            {
                print("I'm right");
                RotateRight();
            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            endTime = Time.time;
        }

        swipeTime = endTime - startTime;

    }

    private void LateUpdate()
    {
        AnimatorStateInfo currentAnimationInfo = animator.GetCurrentAnimatorStateInfo(0);
        float playBackTime = currentAnimationInfo.normalizedTime * currentAnimationInfo.length;

        if (playBackTime > 0f)
        {
            setAnimationStartTime = playBackTime;
        }

    }

    public void RotateRight()
    {

        if (panelRotation.eulerAngles.x == 0)
        {
            isTurningRight = true;
            Invoke("SetBoolFalse", swipeTime);
        }
        else { 
            animator.speed = 1; 
            animator.Play("TurnAllRight", 0, setAnimationStartTime);
            Invoke("PauseAnimation", swipeTime);

        }

        
    }

    public void RotateLeft()
    {
        if (panelRotation.eulerAngles.x == 0)
        {
            isTurningLeft = true;
            Invoke("SetBoolFalse", swipeTime);
        }
        else { 
            animator.speed = 1;
            animator.Play("TurnAllRight", 0, setAnimationStartTime);
            Invoke("PauseAnimation", swipeTime);
        }

    }

    private void SetBoolFalse()
    {
        isTurningRight = false;
        isTurningLeft = false;
    }

    private void PauseAnimation() {
        print("isUsing");
        animator.speed = 0;
    }

}
