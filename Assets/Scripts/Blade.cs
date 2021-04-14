using UnityEngine;

public class Blade : MonoBehaviour
{
	[SerializeField] GameObject bladeTrailPrefab;
	[SerializeField] float minMouseCuttingVelocity = .0001f;
	[SerializeField] float minTouchCuttingVelocity = .01f;

	bool isCutting = false;

	Vector2 previousPosition;
	Vector2 startingTouch;

	GameObject currentBladeTrail;
	Rigidbody2D rb;
	Camera cam;
	CircleCollider2D circleCollider;

	void Start()
	{
		cam = Camera.main;
		rb = this.GetComponent<Rigidbody2D>();
		circleCollider = this.GetComponent<CircleCollider2D>();
	}

	void Update()
	{

#if UNITY_EDITOR || UNITY_STANDALONE

		// Use mouse input in editor or computer
		MouseUpdateCut();

		if (Input.GetMouseButtonDown(0))
		{
			MouseStartCutting();
		}
		else if (Input.GetMouseButtonUp(0))
		{
			MouseStopCutting();
		}
#else
		// Use touch input on mobile
		if (Input.touchCount == 1)
		{
			TouchUpdateCut(Input.GetTouch(0).position);

			// Input check is AFTER TouchUpdateCut, that way if TouchPhase.Ended happened a single frame after the Began Phase
			// a swipe can still be registered (otherwise, isCutting will be set to false and the test wouldn't happen for that began-Ended pair)
			if (Input.GetTouch(0).phase == TouchPhase.Began)
			{
				TouchStartCutting();
			}
			else if (Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				TouchStopCutting();
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
			Vector2 diff = touchPos - startingTouch;

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

	public void TouchStartCutting()
	{
		isCutting = true;
		startingTouch = Input.GetTouch(0).position;

		Vector2 newPosition = cam.ScreenToWorldPoint(Input.GetTouch(0).position);
		this.gameObject.transform.position = newPosition;
		previousPosition = newPosition;

		currentBladeTrail = Instantiate(bladeTrailPrefab, this.transform);
	}

	public void TouchStopCutting()
	{
		isCutting = false;
		circleCollider.enabled = false;

		currentBladeTrail.transform.SetParent(null);
		Destroy(currentBladeTrail, 2f);
	}

    #region MouseInput
    void MouseUpdateCut()
	{
		Vector2 newPosition = cam.ScreenToWorldPoint(Input.mousePosition);
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

	public void MouseStartCutting()
	{
		isCutting = true;

		Vector2 newPosition = cam.ScreenToWorldPoint(Input.mousePosition);
		this.gameObject.transform.position = newPosition;
		previousPosition = newPosition;

		currentBladeTrail = Instantiate(bladeTrailPrefab, this.transform);
	}

	public void MouseStopCutting()
	{
		isCutting = false;
		circleCollider.enabled = false;

		currentBladeTrail.transform.SetParent(null);
		Destroy(currentBladeTrail, 2f);
	}
    #endregion MouseInput
}
