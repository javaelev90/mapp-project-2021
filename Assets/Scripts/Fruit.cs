using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
	[SerializeField] GameObject fruitSlicedPrefab;
	
	[SerializeField] Animator ringAnimator;

	float beatTime;
	AudioSource audioSource;
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Blade")
		{
			Vector3 direction = (col.transform.position - transform.position).normalized;

			//Quaternion rotation = Quaternion.LookRotation(direction);

			GameObject slicedFruit = Instantiate(fruitSlicedPrefab, transform.position, Quaternion.identity);
			print("Swipe diff in time:" + (audioSource.time - beatTime));
			Destroy(slicedFruit, 3f);
			Destroy(this.gameObject);
		}
	}

	public void Initiate(float countDownValue, AudioSource audioSource, float beatTime)
	{
		AnimatorClipInfo[] currentClipInfo = ringAnimator.GetCurrentAnimatorClipInfo(0);
        float currentClipLength = currentClipInfo[0].clip.length;
		float animatorSpeed = (1 / (countDownValue / currentClipLength));
		ringAnimator.speed = animatorSpeed;
		this.audioSource = audioSource;
		this.beatTime = beatTime;
	}
}
