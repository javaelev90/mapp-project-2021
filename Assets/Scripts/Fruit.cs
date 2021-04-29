using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
	[SerializeField] GameObject fruitSlicedPrefab;
	
	[SerializeField] SpriteRenderer timingCircle;
	[SerializeField] Color initialTimingCircleColor;
	[SerializeField] Color finalTimingCircleColor;
	[SerializeField] Animator ringAnimator;
	[SerializeField] AnimationClip ringAnimationClip;
	[SerializeField] AudioClip soundEffect;

	float swipeTimer = default;
	bool hasReducedHP = false;
	float beatTime;
	AudioSource audioSource;
	AudioSource audioSourceSFX;
	Camera mainCamera;
	GameManager gameManager;

	float calibratedAnimationDelay = 0f;

	void Start()
	{
		calibratedAnimationDelay = SongHandler.Instance.GetAudioLatency();
		Debug.Log("Calibrated sound latency: "+calibratedAnimationDelay);
		mainCamera = Camera.main;
		gameManager = GameManager.Instance;
		timingCircle.color = initialTimingCircleColor;
	}

	void Update()
	{
		UpdateTimingCircleGraphics();
		UpdateSwipeTimer();
		if (swipeTimer > 0f && IsOutsideScreen())
			ReduceHP();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Blade")
		{
			GameObject slicedFruit = Instantiate(fruitSlicedPrefab, transform.position, Quaternion.identity);
			
			Debug.Log("Swipe diff in time:" + ((audioSource.time - SongHandler.Instance.GetAudioLatency()) - beatTime));

			float timing = (audioSource.time - calibratedAnimationDelay) - beatTime;
			GameManager.Instance.SetScore(timing);

			// Debug.Log("Swipe diff in time:" + ((audioSource.time ) - beatTime));
			// audioSourceSFX.PlayOneShot(soundEffect);
			if (Mathf.Abs(timing) > GameManager.MAX_SWIPE_TIMING)
			{
				ReduceHP();
			}
			Destroy(slicedFruit, 3f);
			Destroy(this.gameObject);
		}
	}

	public void Initiate(AudioSource audioSource, AudioSource audioSourceSFX, float beatTime)
	{
		float countDown = Mathf.Abs(beatTime - audioSource.time);
		float animatorSpeed =  1 / ((countDown + calibratedAnimationDelay) / ringAnimationClip.length);
		this.audioSource = audioSource;
		this.beatTime = beatTime;
		this.audioSourceSFX = audioSourceSFX;
		ringAnimator.speed = animatorSpeed;
		ringAnimator.SetBool("ShrinkCircle", true);
	}

	void UpdateTimingCircleGraphics()
	{
		float timing = (audioSource.time - calibratedAnimationDelay) - beatTime;
		if(Mathf.Abs(timing) < GameManager.BAD_SWIPE_TIMING_INTERVAL.max)
		{
			timingCircle.color = finalTimingCircleColor;
		}
		if(timing > GameManager.PERFECT_SWIPE_TIMING_INTERVAL.max)
		{
			timingCircle.enabled = false;
		}
	}

	void UpdateSwipeTimer() => swipeTimer = (audioSource.time - SongHandler.Instance.GetAudioLatency()) - beatTime;

	void ReduceHP()
	{
		if (!this.hasReducedHP)
		{
			gameManager.MissedSwipe();
			this.hasReducedHP = true;
		}
	}

	bool IsOutsideScreen()
	{
		Vector3 pos = mainCamera.WorldToScreenPoint(this.transform.position);
		bool outOfBounds = !Screen.safeArea.Contains(pos);
		return outOfBounds;
	}
}
