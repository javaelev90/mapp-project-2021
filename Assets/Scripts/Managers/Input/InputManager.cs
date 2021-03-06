using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

[DefaultExecutionOrder(-2)]
public class InputManager : SingletonPattern<InputManager>
{
    public Vector2 MouseDeltaMove{ get; private set; } = default;

    #region Events
    public delegate void StartTouchEvent(Vector2 position, float time);
    public event StartTouchEvent OnStartTouch;
    public delegate void EndTouchEvent(Vector2 position, float time);
    public event EndTouchEvent OnEndTouch;

    public delegate void StartEnhancedTouchEvent(Finger finger, Vector2 position, float time);
    public event StartEnhancedTouchEvent OnStartEnhancedTouch;
    public delegate void EndEnhancedTouchEvent(Finger finger, Vector2 position, float time);
    public event EndEnhancedTouchEvent OnEndEnhancedTouch;
    public delegate void MoveEnhancedTouchEvent(Finger finger);
    public event MoveEnhancedTouchEvent OnMoveEnhancedTouch;
    #endregion

    PlayerControls playerControls;
    //Camera mainCamera;

    void Awake()
    {
        SetInstanceIfNull(this);
        playerControls = new PlayerControls();
        //mainCamera = Camera.main;
        playerControls.Player.MouseDelta.performed += ctx => MouseDeltaMove = ctx.ReadValue<Vector2>();
    }
    void OnEnable()
    {
        // Enhanced touch
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += FingerDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp += FingerUp;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerMove += FingerMove;

        playerControls.Enable();
    }

    void Start()
    {
        //playerControls.Touch.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        //playerControls.Touch.PrimaryContact.canceled += ctx => EndTouchPrimary(ctx);

        //playerControls.Player.MouseDelta.performed += ctx => MouseDeltaMove = ctx.ReadValue<Vector2>();
    }

    void OnDisable()
    {
        // Enhanced touch
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= FingerDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp -= FingerUp;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerMove -= FingerMove;

        playerControls.Disable();
    }

    void OnDestroy()
    {
        //playerControls.Touch.PrimaryContact.started -= ctx => StartTouchPrimary(ctx);
        //playerControls.Touch.PrimaryContact.canceled -= ctx => EndTouchPrimary(ctx);

        playerControls.Player.MouseDelta.performed -= ctx => MouseDeltaMove = ctx.ReadValue<Vector2>();
    }


    //public Vector2 PrimaryPosition()
    //{
    //    return mainCamera.ScreenToWorldPoint(playerControls.Touch.PrimaryPosition.ReadValue<Vector2>());
    //}

    //public UnityEngine.Touch PrimaryTouch()
    //{
    //    return playerControls.Touch.PrimaryTouch.ReadValue<UnityEngine.Touch>();
    //}

    //void StartTouchPrimary(InputAction.CallbackContext context)
    //{
    //    OnStartTouch?.Invoke(mainCamera.ScreenToWorldPoint(playerControls.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.startTime);
    //}
    //void EndTouchPrimary(InputAction.CallbackContext context)
    //{
    //    OnEndTouch?.Invoke(mainCamera.ScreenToWorldPoint(playerControls.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.time);
    //}

    void FingerDown(Finger finger)
    {
        OnStartEnhancedTouch?.Invoke(finger, finger.screenPosition, Time.time);
    }
    void FingerUp(Finger finger)
    {
        OnEndEnhancedTouch?.Invoke(finger, finger.screenPosition, Time.time);
    }
    void FingerMove(Finger finger)
    {
        OnMoveEnhancedTouch?.Invoke(finger);
    }

}
