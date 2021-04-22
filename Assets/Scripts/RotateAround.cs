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
    
    private float rotationToTime;
    
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
        //Få information om panelens position.
        panelRotation = panel.transform.rotation;

        animator.SetBool("IsTurningRight", isTurningRight);
        animator.SetBool("IsTurningLeft", isTurningLeft);

        if (Input.GetMouseButtonDown(0)) {
            startTime = Time.time;
            startPosition = Input.mousePosition;

            if (startPosition.x < cylinderScreenPoint.x) {
                print("I'm left");

                //Om panelens position inte är 0, så vill vi kolla längden på animationsklippet utifrån objektets position.
                RotateLeft();
                
            }else if(startPosition.x > cylinderScreenPoint.x) {
                print("I'm right");
                RotateRight();
            }
                
        }

        if (Input.GetMouseButtonUp(0)) {
            endTime = Time.time;
        }
   
        swipeTime = endTime - startTime;
        
    }

    public void RotateRight() {
        //animator.speed = 1;
        isTurningRight = true;
        //isTurningLeft = false;
        Invoke("SetBoolFalse", swipeTime);
    }

    public void RotateLeft() {
        //animator.speed = 1;
        isTurningLeft = true;
        //isTurningRight = false;
        Invoke("SetBoolFalse", swipeTime);
    }

    private void SetBoolFalse() {
        //animator.speed = 0;
        isTurningRight = false;
        isTurningLeft = false;
    }

}
