using UnityEngine.InputSystem;
using UnityEngine;
#if !UNITY_EDITOR && !UNITY_STANDALONE
using UnityEngine.InputSystem.EnhancedTouch;
#endif

public class PanelController : MonoBehaviour
{
    [SerializeField] float mouseRotationSpeed = .1f;

    float rotationSpeed = 0f;
    float rotationSlowdownMultiplier = .95f;

#if UNITY_EDITOR || UNITY_STANDALONE
    PlayerControls input;
    Vector2 mouseDeltaMove;
#else
    InputManager inputManager;
    float touchRotationMultiplier = .1f;
    float touchRotationSpeed = .5f;
#endif

    void Awake()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        input = new PlayerControls();
        input.Player.MouseDelta.performed += ctx => mouseDeltaMove = ctx.ReadValue<Vector2>();
#endif
    }

    void Start()
    {
#if !UNITY_EDITOR && !UNITY_STANDALONE
        inputManager = InputManager.Instance;
        inputManager.OnMoveEnhancedTouch += OnMovingTouch;
        inputManager.OnStartEnhancedTouch += OnStartTouch;
#endif
    }

    void FixedUpdate()
    {
#if UNITY_EDITOR || UNITY_STANDALONE

        if (Mouse.current.leftButton.isPressed)
        {
            this.transform.Rotate(Vector3.down, mouseDeltaMove.x * mouseRotationSpeed);
            rotationSpeed = mouseDeltaMove.x * mouseRotationSpeed;
        }
        else
        {
            this.transform.Rotate(Vector3.down, rotationSpeed);
            rotationSpeed *= rotationSlowdownMultiplier;
        }


#else
        this.transform.Rotate(Vector3.down, rotationSpeed * touchRotationSpeed);
        rotationSpeed *= rotationSlowdownMultiplier;
#endif

    }

    void OnEnable()
    {
#if UNITY_EDITOR || UNITY_STANDALONE

        input.Enable();
#else
        EnhancedTouchSupport.Enable();
#endif

    }

    void OnDisable()
    {
#if UNITY_EDITOR || UNITY_STANDALONE

        input.Disable();
#else
        EnhancedTouchSupport.Disable();
#endif

    }

#if !UNITY_EDITOR && !UNITY_STANDALONE

    void OnMovingTouch(Finger finger)
    {
        if ((finger.currentTouch.startScreenPosition - finger.currentTouch.screenPosition).magnitude > (Screen.width / 20f))
        {
            rotationSpeed = Mathf.Lerp(rotationSpeed, finger.currentTouch.delta.x, Time.deltaTime * touchRotationMultiplier);
        }
    }

    void OnStartTouch(Finger finger, Vector2 startPos, float startTime)
    {
        rotationSpeed = 0f;
    }
#endif

}
