using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class PanelController : MonoBehaviour
{
    [SerializeField] float mouseRotationSpeed = .1f;

    float rotationSpeed = 0f;
    float rotationSlowdownMultiplier = .95f;
    InputManager inputManager;

    float touchRotationMultiplier = .1f;
    float touchRotationSpeed = .75f;

    void OnEnable()
    {
#if !UNITY_EDITOR && !UNITY_STANDALONE
        EnhancedTouchSupport.Enable();
#endif
    }

    void OnDisable()
    {
#if !UNITY_EDITOR && !UNITY_STANDALONE
        EnhancedTouchSupport.Disable();
#endif
    }

    void Start()
    {
        inputManager = InputManager.Instance;

#if !UNITY_EDITOR && !UNITY_STANDALONE
        inputManager.OnMoveEnhancedTouch += OnMovingTouch;
        inputManager.OnStartEnhancedTouch += OnStartTouch;
#endif

    }

    void FixedUpdate()
    {
#if UNITY_EDITOR || UNITY_STANDALONE

        if (Mouse.current.leftButton.isPressed)
        {
            this.transform.Rotate(Vector3.down, inputManager.MouseDeltaMove.x * mouseRotationSpeed);
            rotationSpeed = inputManager.MouseDeltaMove.x * mouseRotationSpeed;
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



    void OnDestroy()
    {
#if !UNITY_EDITOR && !UNITY_STANDALONE
        inputManager.OnMoveEnhancedTouch -= OnMovingTouch;
        inputManager.OnStartEnhancedTouch -= OnStartTouch;
#endif
    }

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

}
