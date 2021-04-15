using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
	[SerializeField] GameObject fruitSlicedPrefab;
	
	[SerializeField] Animator ringAnimator;
	[SerializeField] AnimationClip ringAnimationClip;
	float beatTime;
	AudioSource audioSource;
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Blade")
		{
			Vector3 direction = (col.transform.position - transform.position).normalized;

			//Quaternion rotation = Quaternion.LookRotation(direction);

			GameObject slicedFruit = Instantiate(fruitSlicedPrefab, transform.position, Quaternion.identity);
			Debug.Log("Swipe diff in time:" + (audioSource.time - beatTime));
			Destroy(slicedFruit, 3f);
			Destroy(this.gameObject);
		}
	}

	public void Initiate(float countDownValue, AudioSource audioSource, float beatTime)
	{

		float countDown = Mathf.Abs(beatTime - audioSource.time);
		float animatorSpeed =  1 / (countDown / ringAnimationClip.length);
		this.audioSource = audioSource;
		this.beatTime = beatTime;
		ringAnimator.speed = animatorSpeed;
		ringAnimator.SetBool("ShrinkCircle", true);

	}
}
