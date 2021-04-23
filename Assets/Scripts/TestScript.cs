using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{


    private Animator animator;
    public GameObject panel;
    private Quaternion panelRotation;

    private float startTime;
    private float endTime;
    private float swipeTime;

    private float rotationToTime;
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
        //F� information om panelens position.
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

                //Om panelens position inte �r 0, s� vill vi kolla l�ngden p� animationsklippet utifr�n objektets position.
                //Anv�nd sedan l�ngden p� klippet, och konvertera det till den andra animationen.
                //Starta den andra animationen fr�n den positionen.
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

        if (playBackTime > 0f) {
            setAnimationStartTime = playBackTime;
        }

    }

    public void RotateRight()
    {
        //animator.speed = 1;

        //Play animation from certain time
        animator.Play("TurnAllRight", 0, setAnimationStartTime);

        //isTurningRight = true;
        //isTurningLeft = false;
        Invoke("SetBoolFalse", swipeTime);
    }

    public void RotateLeft()
    {
        //animator.speed = 1;
        isTurningLeft = true;
        //isTurningRight = false;
        Invoke("SetBoolFalse", swipeTime);
    }

    private void SetBoolFalse()
    {
        //animator.speed = 0;
        isTurningRight = false;
        isTurningLeft = false;
    }

}