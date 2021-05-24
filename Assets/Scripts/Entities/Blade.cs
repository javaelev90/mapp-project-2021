using UnityEngine;
using UnityEngine.InputSystem;
#if !UNITY_EDITOR && !UNITY_STANDALONE
using UnityEngine.InputSystem.EnhancedTouch;
#endif

// TODO: Create object pooling for bladeTrail
public class Blade : MonoBehaviour
{
	[SerializeField] GameObject bladeTrailPrefab;
	[Tooltip("How fast the player need to move the mouse for it to be registered, 0 = clicking enabled")]
	[SerializeField] float minMouseCuttingVelocity = .0001f;
	[Tooltip("How fast the player need to swipe for it to be registered, 0 = tapping enabled")]
	[SerializeField] float minTouchCuttingVelocity = .01f;
	[SerializeField] LayerMask sliceLayer;

	public delegate void FruitSlicedEvent(Collider2D fruitCollider);
	public event FruitSlicedEvent OnFruitSliced;

	GameObject currentBladeTrail;
	Rigidbody2D rb;
	Camera cam;
	CircleCollider2D circleCollider;

#region Input
    Vector2 previousPosition;
	Vector2 startingTouch;
	bool cuttingInitiated = false;
	RaycastHit2D[] collisionBuffer = new RaycastHit2D[12];

#if !UNITY_EDITOR && !UNITY_STANDALONE
	InputManager inputManager;
#endif

	#endregion Input

	void Awake()
	{
		cam = Camera.main;
		rb = this.GetComponent<Rigidbody2D>();
		circleCollider = this.GetComponent<CircleCollider2D>();

#if !UNITY_EDITOR && !UNITY_STANDALONE
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
		if (!GameManager.Instance.GameIsPaused)
		{
			if (Mouse.current.leftButton.wasPressedThisFrame)
			{
				MouseStartCutting(Mouse.current);
			}
			else if (Mouse.current.leftButton.wasReleasedThisFrame)
			{
				MouseStopCutting();
			}
		}
#endif

	}

	void FixedUpdate()
	{
#if UNITY_EDITOR || UNITY_STANDALONE
		//Use mouse input in editor or computer
		if (!GameManager.Instance.GameIsPaused)
		{
			MouseUpdateCut(Mouse.current);
		}
#else
		//Use touch input on mobile
		if (!GameManager.Instance.GameIsPaused)
		{
			if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count == 1)
			{
				TouchUpdateCut(UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition);
			}
		}
#endif
	}
	#region Touch Input

	void TouchUpdateCut(Vector2 touchPos)
	{
		Vector2 newPosition = cam.ScreenToWorldPoint(touchPos);
		rb.position = newPosition;

		float velocity = (newPosition - previousPosition).magnitude * Time.deltaTime;
		if (cuttingInitiated && velocity > minTouchCuttingVelocity)
		{
			Cut(newPosition);
		}

		previousPosition = newPosition;
	}

	public void TouchStartCutting(Vector2 touchPos, float time)
	{
		cuttingInitiated = true;
		startingTouch = touchPos;
		Vector2 newPosition = cam.ScreenToWorldPoint(touchPos);
		this.transform.position = newPosition;
		previousPosition = newPosition;

		currentBladeTrail = Instantiate(bladeTrailPrefab, this.transform.position, Quaternion.identity, this.transform); // Creating new instances to stop sudden removal of trails, TODO: make object pool
	}

	public void TouchStopCutting(Vector2 position, float time)
	{
		cuttingInitiated = false;

		currentBladeTrail.transform.SetParent(null);
		Destroy(currentBladeTrail, .5f);
	}
#endregion Touch Input

#region Mouse Input
    void MouseUpdateCut(Mouse currentMouse)
	{
		Vector2 newPosition = cam.ScreenToWorldPoint(new Vector3(currentMouse.position.x.ReadValue(), currentMouse.position.y.ReadValue(), 0f));
		rb.position = newPosition;

		float velocity = (newPosition - previousPosition).magnitude * Time.deltaTime;
		if (cuttingInitiated && velocity > minMouseCuttingVelocity)
		{
			Cut(newPosition);
		}

		previousPosition = newPosition;
	}

	public void MouseStartCutting(Mouse currentMouse)
	{
		cuttingInitiated = true;

		Vector2 newPosition = cam.ScreenToWorldPoint(new Vector3(currentMouse.position.x.ReadValue(), currentMouse.position.y.ReadValue(), 0f));
		this.transform.position = newPosition;
		previousPosition = newPosition;

		currentBladeTrail = Instantiate(bladeTrailPrefab, this.transform.position, Quaternion.identity, this.transform);
	}

	public void MouseStopCutting()
	{
		cuttingInitiated = false;
		if(currentBladeTrail != null){
			currentBladeTrail?.transform.SetParent(null); // Denna ger error ibland men jag tror att det har att g�ra med att man spelar i Editor och f�r musen utanf�r spelsk�rmen osv.
			Destroy(currentBladeTrail, .5f);
		}
	}
#endregion Mouse Input

	void Cut(Vector2 newPosition)
	{
		Vector2 direction = (newPosition - previousPosition).normalized;
		float distance = Vector2.Distance(newPosition, previousPosition);
		if (Physics2D.CircleCastNonAlloc(previousPosition, circleCollider.radius, direction, collisionBuffer, distance, sliceLayer) > 0)
		{
			for (int i = 0; i < collisionBuffer.Length; i++)
			{
				if (collisionBuffer[i].collider == circleCollider || collisionBuffer[i].collider == null)
					continue;

				OnFruitSliced?.Invoke(collisionBuffer[i].collider);
			}
		}
	}
}
