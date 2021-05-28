//using UnityEngine;
//using UnityEngine.InputSystem.EnhancedTouch;

///// <summary>
///// This was just a test script....
///// </summary>
//public class EnhancedBlade : MonoBehaviour
//{
//	[SerializeField] GameObject bladeTrailPrefab;
//	[Tooltip("How fast the player need to move the mouse for it to be registered, 0 = clicking enabled")]
//	[SerializeField] float minMouseCuttingVelocity = .0001f;
//	[Tooltip("How fast the player need to swipe for it to be registered, 0 = tapping enabled")]
//	[SerializeField] float minTouchCuttingVelocity = .01f;

//	bool isCutting = false;

//	Vector2 previousPosition;
//	Vector2 startingTouch;

//	GameObject currentBladeTrail;
//	Rigidbody2D rb;
//	Camera cam;
//	CircleCollider2D circleCollider;
//	//InputManager inputManager;
	

//	void Awake()
//	{
//		cam = Camera.main;
//		rb = this.GetComponent<Rigidbody2D>();
//		circleCollider = this.GetComponent<CircleCollider2D>();
//		//inputManager = InputManager.Instance;
//		EnhancedTouchSupport.Enable();
//		//TouchSimulation.Enable();
//	}
//	private void OnEnable()
//	{
//		//inputManager.OnStartEnhancedTouch += TouchStartCutting;
//	}
//	void OnDestroy()
//	{
//		EnhancedTouchSupport.Disable();
//		//TouchSimulation.Disable();
//	}

//	void Update()
//	{
//		if (!GameManager.Instance.GameIsPaused)
//		{
//			if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count == 1)
//			{
//				TouchUpdateCut(UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition);

//				// Input check is AFTER TouchUpdateCut, that way if TouchPhase.Ended happened a single frame after the Began Phase
//				// a swipe can still be registered (otherwise, cuttingInitiated will be set to false and the test wouldn't happen for that began-Ended pair)
//				if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
//				{
//					TouchStartCutting(UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition);
//				}
//				else if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Ended)
//				{
//					TouchStopCutting();
//				}
//			}
//		}

//	}
	
//	void TouchUpdateCut(Vector2 touchPos)
//	{
//		Vector2 newPosition = cam.ScreenToWorldPoint(touchPos);
//		rb.position = newPosition;

//		if (isCutting)
//		{
//			Vector2 diff = touchPos - startingTouch; // Could be wrong if the player continuosly swipe the screen without lifting...

//			// Put difference in Screen ratio, but using only width, so the ratio is the same on both
//			// axes (otherwise we would have to swipe more vertically...)
//			diff = new Vector2(diff.x / Screen.width, diff.y / Screen.width);

//			if (diff.magnitude > minTouchCuttingVelocity) //trigger cutting if moving fast enough
//				circleCollider.enabled = true;
//			else
//				circleCollider.enabled = false;
//		}

//		previousPosition = newPosition;

//	}

//	public void TouchStartCutting(Vector2 touchPos)
//	{
//		isCutting = true;
//		startingTouch = touchPos;
//		Vector2 newPosition = cam.ScreenToWorldPoint(touchPos);
//		this.gameObject.transform.position = newPosition;
//		previousPosition = newPosition;

//		currentBladeTrail = Instantiate(bladeTrailPrefab, this.transform);
//	}

//	public void TouchStopCutting()
//	{
//		isCutting = false;
//		circleCollider.enabled = false;

//		currentBladeTrail?.transform.SetParent(null);
//		Destroy(currentBladeTrail, 1f);
//	}

//#region MouseInput
// //   void MouseUpdateCut(Mouse currentMouse)
//	//{
//	//	Vector2 newPosition = cam.ScreenToWorldPoint(new Vector3(currentMouse.position.x.ReadValue(), currentMouse.position.y.ReadValue(), 0f));
//	//	rb.position = newPosition;

//	//	if (cuttingInitiated)
//	//	{
//	//		float velocity = (newPosition - previousPosition).magnitude * Time.deltaTime;

//	//		if (velocity > minMouseCuttingVelocity)
//	//			circleCollider.enabled = true;
//	//		else
//	//			circleCollider.enabled = false;
//	//	}

//	//	previousPosition = newPosition;

//	//}

//	//public void MouseStartCutting(Mouse currentMouse)
//	//{
//	//	cuttingInitiated = true;

//	//	Vector2 newPosition = cam.ScreenToWorldPoint(new Vector3(currentMouse.position.x.ReadValue(), currentMouse.position.y.ReadValue(), 0f));

//	//	print("mouse raw x: " + currentMouse.position.x.ReadValue() + " y: " + currentMouse.position.y.ReadValue());
//	//	print("screenToWorldPoint: " + newPosition);

//	//	this.gameObject.transform.position = newPosition;
//	//	previousPosition = newPosition;

//	//	currentBladeTrail = Instantiate(bladeTrailPrefab, this.transform);
//	//}

//	//public void MouseStopCutting()
//	//{
//	//	cuttingInitiated = false;
//	//	circleCollider.enabled = false;

//	//	currentBladeTrail?.transform.SetParent(null);
//	//	Destroy(currentBladeTrail, 2f);
//	//}
//#endregion MouseInput
//}
