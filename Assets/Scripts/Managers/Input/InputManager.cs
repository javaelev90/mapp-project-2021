using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

[DefaultExecutionOrder(-1)]
public class InputManager : SingletonPattern<InputManager>
{
    #region Events
    public delegate void StartTouchEvent(Vector2 position, float time);
    public event StartTouchEvent OnStartTouch;
    public delegate void EndTouchEvent(Vector2 position, float time);
    public event EndTouchEvent OnEndTouch;

    public delegate void StartEnhancedTouchEvent(Vector2 position, float time);
    public event StartEnhancedTouchEvent OnStartEnhancedTouch;
    public delegate void EndEnhancedTouchEvent(Vector2 position, float time);
    public event EndEnhancedTouchEvent OnEndEnhancedTouch;
    #endregion

    PlayerControls playerControls;
    Camera mainCamera;

    void Awake()
    {
        playerControls = new PlayerControls();
        mainCamera = Camera.main;
    }
    void OnEnable()
    {
        // Enhanced touch
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += FingerDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp += FingerUp;

        playerControls.Enable();
    }
    void OnDisable()
    {
        // Enhanced touch
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= FingerDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp -= FingerUp;

        playerControls.Disable();
    }

    void Start()
    {
        playerControls.Touch.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        playerControls.Touch.PrimaryContact.canceled += ctx => EndTouchPrimary(ctx);
    }

    void StartTouchPrimary(InputAction.CallbackContext context)
    {
        OnStartTouch?.Invoke(mainCamera.ScreenToWorldPoint(playerControls.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.startTime);
    }
    void EndTouchPrimary(InputAction.CallbackContext context)
    {
        OnEndTouch?.Invoke(mainCamera.ScreenToWorldPoint(playerControls.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.time);
    }

    public Vector2 PrimaryPosition()
    {
        return mainCamera.ScreenToWorldPoint(playerControls.Touch.PrimaryPosition.ReadValue<Vector2>());
    }

    void FingerDown(Finger finger)
    {
        OnStartEnhancedTouch?.Invoke(finger.screenPosition, Time.time);
    }
    void FingerUp(Finger finger)
    {
        OnEndEnhancedTouch?.Invoke(finger.screenPosition, Time.time);
    }
}
