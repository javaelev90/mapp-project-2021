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
	Vector3 target;
	float timeElapsed = 0f;
	float calibratedAnimationDelay = 0f;
	Vector2 startPosition;
	float timeToTargetPosition;
	bool hasReachedTarget = false;
	SoulSpawner.SpawnPattern spawnPattern;
	void Start()
	{
		calibratedAnimationDelay = SongHandler.Instance.GetAudioLatency();
		mainCamera = Camera.main;
		gameManager = GameManager.Instance;
		timingCircle.color = new Color(initialTimingCircleColor.r,initialTimingCircleColor.b,initialTimingCircleColor.g,0.2f);//initialTimingCircleColor;
	}

	void Update()
	{
		UpdateTimingCircleGraphics();
		UpdateSwipeTimer();
		if (swipeTimer > 0f && IsOutsideScreen())
			ReduceHP();
		float timing = (audioSource.time - calibratedAnimationDelay) - beatTime;

		if(spawnPattern == SoulSpawner.SpawnPattern.LINEAR && !hasReachedTarget)
		{
			if(timeElapsed/timeToTargetPosition <= 1f){
				transform.position = Vector3.Lerp(startPosition, target, timeElapsed/timeToTargetPosition);
				timeElapsed += Time.deltaTime;
			} else {
				hasReachedTarget = true;
				GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			} 
		}
		if(spawnPattern == SoulSpawner.SpawnPattern.TRIANGLE ||
			spawnPattern == SoulSpawner.SpawnPattern.SQUARE)
		{
			if(Vector3.Distance(transform.position, target) <= 0.3f && timing <= 0){
				GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
			} else {
				GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
			}
		}

	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Blade")
		{
			GameObject slicedFruit = Instantiate(fruitSlicedPrefab, transform.position, Quaternion.identity);
			
			// Debug.Log("Swipe diff in time:" + ((audioSource.time - SongHandler.Instance.GetAudioLatency()) - beatTime));

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

	public void Initiate(AudioSource audioSource, AudioSource audioSourceSFX, float beatTime, Vector3 target, Vector3 velocity, SoulSpawner.SpawnPattern spawnPattern)
	{
		this.target = target;
		float countDown = Mathf.Abs(beatTime - audioSource.time);
		float animatorSpeed =  1 / ((countDown + calibratedAnimationDelay) / ringAnimationClip.length);
		this.audioSource = audioSource;
		this.beatTime = beatTime;
		this.audioSourceSFX = audioSourceSFX;
		this.spawnPattern = spawnPattern;
		this.timeToTargetPosition = beatTime - (audioSource.time - calibratedAnimationDelay);
		this.startPosition = transform.position;
		// if(Vector2.Distance(this.startPosition, this.target) > 4){
		// 	this.timeToTargetPosition += 0.7f;
		// 	// Debug.Log("long distance");
		// } else if(Vector2.Distance(this.startPosition, this.target) < 2){
		// 	this.timeToTargetPosition -= 0.2f;
		// 	// Debug.Log("short distance");
		// }
		ringAnimator.speed = animatorSpeed;
		ringAnimator.SetBool("ShrinkCircle", true);
		GetComponent<Rigidbody2D>().velocity = velocity;
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
