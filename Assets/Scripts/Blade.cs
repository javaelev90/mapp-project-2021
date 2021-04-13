using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
	[SerializeField] GameObject bladeTrailPrefab;
	[SerializeField] float minCuttingVelocity = .0001f;

	bool isCutting = false;

	Vector2 previousPosition;

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
		if (isCutting)
		{
			UpdateCut();
		}
	}

	void UpdateCut()
	{
		Vector2 newPosition = cam.ScreenToWorldPoint(Input.mousePosition);
		rb.position = newPosition;
		print("newPos: " + newPosition + " - mousePos: " + Input.mousePosition);

		float velocity = (newPosition - previousPosition).magnitude * Time.deltaTime;

		if (velocity > minCuttingVelocity)
		{
			circleCollider.enabled = true;
		}
		else
		{
			circleCollider.enabled = false;
		}

		previousPosition = newPosition;
	}

	public void StartCutting()
	{
		isCutting = true;

		Vector2 newPosition = cam.ScreenToWorldPoint(Input.mousePosition);
		this.gameObject.transform.position = newPosition;
		previousPosition = newPosition;

		currentBladeTrail = Instantiate(bladeTrailPrefab, this.transform);
	}

	public void StopCutting()
	{
		isCutting = false;
		circleCollider.enabled = false;

		currentBladeTrail.transform.SetParent(null);
		Destroy(currentBladeTrail, 2f);
	}
}
