using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
	[SerializeField] GameObject fruitSlicedPrefab;

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Blade")
		{
			Vector3 direction = (col.transform.position - transform.position).normalized;

			//Quaternion rotation = Quaternion.LookRotation(direction);

			GameObject slicedFruit = Instantiate(fruitSlicedPrefab, transform.position, Quaternion.identity);
			Destroy(slicedFruit, 3f);
			Destroy(this.gameObject);
		}
	}
}
