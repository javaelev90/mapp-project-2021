using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class RotateAround1 : MonoBehaviour
{
    private PlayerControls input;

    //private Animator animator;

    public GameObject[] panels;

    public float rotationSpeed = 2f;

    private Vector3 panelPosition;
    private Quaternion startRotation;

    private Quaternion panelRotation;

    private float movementDirection;
    Vector2 mouseDeltaMove;

    Vector3 rotationUp = new Vector3(0, 1, 0);

    private bool checkInput = true;

    private void Awake()
    { 
        //lurpar till vissa positioner när children befinner sig på 000 roation 
        input = new PlayerControls();
        input.Player.MouseDelta.performed += ctx => mouseDeltaMove = ctx.ReadValue<Vector2>();
        
    }
    void Start()
    {
        //animator = gameObject.GetComponent<Animator>();
        //animator.speed = 1;
        movementDirection = 0;
    }
    void Update()
    {
        
        //float distance = 1f/ (panels.Length - 1f);
        //this.transform.Rotate(rotationUp);
        
        checkInput = true;
        //this.transform.RotateAround(this.transform.parent.position, this.transform.parent.up, 90f * Time.deltaTime);
        //panelRotation = panel.transform.rotation;
        //currentEulerAngles +=

        if (Mouse.current.leftButton.isPressed)
        {
            print("Mouse is moving in this X-axis: " + mouseDeltaMove.x);
            //animator.speed = 1;

            //Rotera höger eller vänster?
            //Slerp - panel
            if (mouseDeltaMove.x < 0)
            {
                RotateLeft();
            }
            else if (mouseDeltaMove.x > 0)
            {
                RotateRight();
            }

        }

        //if(!checkInput){

           // SlerpPanel();
        //}


        //Check panel positions
        //Rotate cylinder
        //lerp cylinder so panel poisition between 40 and -40 become centered.



        /*
        for(int i = 0; i < panels.Length; i++){
            panelPosition = panels[i].transform.localEulerAngles;
            panelRotation = panels[i].transform.rotation;
        
            //Vector3 panelPosition = panels[i].transform.eulerAngles;
            if(panelPosition.y <= 40 && panelPosition.y >= -40){
                panels[i].transform.position = Vector3.Lerp(panels[i].transform.position, new Vector3(0f, 0f, -3.01f), Time.deltaTime * 10f);
                panels[i].transform.rotation = Quaternion.Lerp(panelRotation, startRotation, Time.deltaTime * 10f);
            
                //panels[i].transform.rotation = startPosition;
                
            }
            
            
           // float startTime = Time.time;
            //    panels[i].GetComponent<RectTransform>();
           //     transform.position = Vector3.Lerp(panels[i].transform.position, new Vector3(0f, 0f, -3.01f), Time.time * 0.5f);
        }
        /*
        foreach(GameObject panel in panels){
            panelRotation = panel.transform.rotation;
            if(panelRotation.y == 0f){
                print("banan");
            }
        }
        */
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
        checkInput = false;
        //this.transform.rotation = Quaternion.Slerp(this.transform.rotation, startRotation, Time.deltaTime);
        //this.transform.Rotate(-rotationUp);
        
        //this.transform.RotateAround(new Vector3(0, 0, 621),
        //Doesnt work but rotates the thing kinda cooly
        //this.transform.eulerAngles = new Vector3(40, 0, 0);
    /*
        foreach(GameObject panel in panels){
            panel.transform.RotateAround(panel.transform.parent.position, -Vector3.up, 120f * Time.deltaTime);
        }
        */
        
        
        //this.transform.RotateAround(transform.panel.position, -this.transform.parent.up, 90f * Time.deltaTime);
        //this.transform.eulerAngles = currentEulerAngles;
        //PlayClip("TurnAllRight", 360 - (panelRotation.eulerAngles.y / 360));
        
    }

    public void RotateLeft()
    {   
        checkInput = false;
        //this.transform.rotation = Quaternion.Slerp(this.transform.rotation, startRotation, Time.deltaTime);
        this.transform.Rotate(rotationUp);
        /*
        foreach(GameObject panel in panels){
            panel.transform.RotateAround(panel.transform.parent.position, Vector3.up, 120f * Time.deltaTime);
        }
        */
       
        //this.transform.RotateAround(this.transform.parent.position, this.transform.parent.up, 90f * Time.deltaTime);
        //panel.transform.RotateAround(transform.parent.position, transform.parent.up, 90f * Time.deltaTime);
        //PlayClip("TurnAllLeft", panelRotation.eulerAngles.y / 360);

    }

    public void SlerpPanel(){
        
        //Slerp cylinder
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, startRotation, Time.deltaTime);


    }
/*
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
    */

}
