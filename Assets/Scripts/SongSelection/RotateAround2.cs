using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;

public class RotateAround2 : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 1f;

    PlayerControls input;
    Vector2 mouseDeltaMove;

    void Awake()
    {
        input = new PlayerControls();
        input.Player.MouseDelta.performed += ctx => mouseDeltaMove = ctx.ReadValue<Vector2>();
    }

    void Start()
    {

    }

    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            //print("Mouse is moving in this X-axis: " + mouseDeltaMove.x);
            this.transform.Rotate(Vector3.down, mouseDeltaMove.x * rotationSpeed);
        }
        
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

}
