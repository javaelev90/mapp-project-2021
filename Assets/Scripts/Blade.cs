using UnityEngine;
using UnityEngine.InputSystem;
#if !UNITY_EDITOR && !UNITY_STANDALONE
using UnityEngine.InputSystem.EnhancedTouch;
#endif

public class Blade : MonoBehaviour
{
	[SerializeField] GameObject bladeTrailPrefab;
	[Tooltip("How fast the player need to move the mouse for it to be registered, 0 = clicking enabled")]
	[SerializeField] float minMouseCuttingVelocity = .0001f;
	[Tooltip("How fast the player need to swipe for it to be registered, 0 = tapping enabled")]
	[SerializeField] float minTouchCuttingVelocity = .01f;

	GameObject currentBladeTrail;
	Rigidbody2D rb;
	Camera cam;
	CircleCollider2D circleCollider;

#region Input
    Vector2 previousPosition;
	Vector2 startingTouch;
	bool isCutting = false;

#if UNITY_EDITOR || UNITY_STANDALONE
	Mouse currentMouse;
#else
	InputManager inputManager;
#endif
#endregion Input

    void Awake()
	{
		cam = Camera.main;
		rb = this.GetComponent<Rigidbody2D>();
		circleCollider = this.GetComponent<CircleCollider2D>();

#if UNITY_EDITOR || UNITY_STANDALONE
		currentMouse = Mouse.current;
#else
		inputManager = InputManager.Instance;
		EnhancedTouchSupport.Enable();
#endif

	}
	void OnEnable()
	{
#if !UNITY_EDITOR && !UNITY_STANDALONE
		inputManager.OnStartEnhancedTouch += TouchStartCutting;
		inputManager.OnEndEnhancedTouch += TouchStopCutting;
#endif
	}
	void OnDisable()
	{
#if !UNITY_EDITOR && !UNITY_STANDALONE
		inputManager.OnStartEnhancedTouch -= TouchStartCutting;
		inputManager.OnEndEnhancedTouch -= TouchStopCutting;
#endif
	}
	void OnDestroy()
	{
#if !UNITY_EDITOR && !UNITY_STANDALONE
		EnhancedTouchSupport.Disable();
#endif
	}

	void Update()
	{
#if UNITY_EDITOR || UNITY_STANDALONE
		//Use mouse input in editor or computer
		if (!InGameMenuController.GameIsPaused)
		{
			MouseUpdateCut(currentMouse);

			if (currentMouse.leftButton.wasPressedThisFrame)
			{
				MouseStartCutting(currentMouse);
			}
			else if (currentMouse.leftButton.wasReleasedThisFrame)
			{
				MouseStopCutting();
			}
		}
#else
		// Use touch input on mobile
		if (!InGameMenuController.gameIsPaused)
		{
			if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count == 1)
			{
				TouchUpdateCut(UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition);

				// Input check is AFTER TouchUpdateCut, that way if TouchPhase.Ended happened a single frame after the Began Phase
				// a swipe can still be registered (otherwise, isCutting will be set to false and the test wouldn't happen for that began-Ended pair)
				if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
				{
					TouchStartCutting(UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition, 
						(float)UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].startTime);
				}
				else if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Ended)
				{
					TouchStopCutting(UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition, 
						(float)UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].time);
				}
			}
		}
#endif
	}

	void TouchUpdateCut(Vector2 touchPos)
	{
		Vector2 newPosition = cam.ScreenToWorldPoint(touchPos);
		rb.position = newPosition;

		if (isCutting)
		{
			Vector2 diff = touchPos - startingTouch; // TODO: Could be wrong if the player continuosly swipe the screen without lifting...

			// Put difference in Screen ratio, but using only width, so the ratio is the same on both
			// axes (otherwise we would have to swipe more vertically...)
			diff = new Vector2(diff.x / Screen.width, diff.y / Screen.width);

			if (diff.magnitude > minTouchCuttingVelocity) //trigger cutting if moving fast enough
				circleCollider.enabled = true;
			else
				circleCollider.enabled = false;
		}

		previousPosition = newPosition;
	}

	public void TouchStartCutting(Vector2 touchPos, float time)
	{
		isCutting = true;
		startingTouch = touchPos;
		Vector2 newPosition = cam.ScreenToWorldPoint(touchPos);
		this.gameObject.transform.position = newPosition;
		previousPosition = newPosition;

		currentBladeTrail = Instantiate(bladeTrailPrefab, this.transform);
	}

	public void TouchStopCutting(Vector2 position, float time)
	{
		isCutting = false;
		circleCollider.enabled = false;

		currentBladeTrail.transform.SetParent(null);
		Destroy(currentBladeTrail, 2f);
	}

#region MouseInput
    void MouseUpdateCut(Mouse currentMouse)
	{
		Vector2 newPosition = cam.ScreenToWorldPoint(new Vector3(currentMouse.position.x.ReadValue(), currentMouse.position.y.ReadValue(), 0f));
		rb.position = newPosition;

		if (isCutting)
		{
			float velocity = (newPosition - previousPosition).magnitude * Time.deltaTime;

			if (velocity > minMouseCuttingVelocity)
				circleCollider.enabled = true;
			else
				circleCollider.enabled = false;
		}

		previousPosition = newPosition;
	}

	public void MouseStartCutting(Mouse currentMouse)
	{
		isCutting = true;

		Vector2 newPosition = cam.ScreenToWorldPoint(new Vector3(currentMouse.position.x.ReadValue(), currentMouse.position.y.ReadValue(), 0f));
		this.gameObject.transform.position = newPosition;
		previousPosition = newPosition;

		currentBladeTrail = Instantiate(bladeTrailPrefab, this.transform);
	}

	public void MouseStopCutting()
	{
		isCutting = false;
		circleCollider.enabled = false;

		currentBladeTrail?.transform.SetParent(null); // Denna ger error ibland men jag tror att det har att göra med att man spelar i Editor och för musen utanför spelskärmen osv.
		Destroy(currentBladeTrail, 2f);
	}
#endregion MouseInput
}
