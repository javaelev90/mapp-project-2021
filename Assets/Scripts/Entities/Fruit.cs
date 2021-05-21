using UnityEngine;

// TODO: Skapa object pooling för flygande objekt och partiklar
public class Fruit : MonoBehaviour
{
	[Tooltip("The prefab spawned after slicing")]
	[SerializeField] GameObject fruitSlicedPrefab;
	[SerializeField] GameObject fruitMissedSlicePrefab;

	[Header("Timing properties")]
	[SerializeField] SpriteRenderer timingCircle;
	[SerializeField] Color initialTimingCircleColor;
	[SerializeField] Color finalTimingCircleColor;
	[Header("Animation")]
	[SerializeField] Animator ringAnimator;
	[SerializeField] AnimationClip ringAnimationClip;
	[Header("SFX")]
	[SerializeField] AudioClip soundEffect;
	[Header("VFX")]
	[SerializeField] GameObject particleEffectsPerfect;
	[SerializeField] GameObject particleEffectsGood;
	[SerializeField] GameObject particleEffectsBad;
	[SerializeField] GameObject particleEffectsMissed;

	float swipeTimer = default;
	bool hasReducedHP = false;
	float beatTime;
	AudioSource audioSource;
	AudioSource audioSourceSFX;
	Camera mainCamera;
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
		timingCircle.color = new Color(initialTimingCircleColor.r,initialTimingCircleColor.b,initialTimingCircleColor.g,0.2f);//initialTimingCircleColor;
	}

	void Update()
	{
		UpdateTimingCircleGraphics();
		UpdateSwipeTimer();
		if (swipeTimer > GameManager.PERFECT_SWIPE_TIMING_INTERVAL.max && IsOutsideScreen())
		{
			ReduceHP();
		}
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

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Blade" && !this.hasReducedHP)
		{
			float sliceTiming = (audioSource.time - calibratedAnimationDelay) - beatTime;
			GameManager.Instance.SetScore(sliceTiming);
			HandleSliceEffects(sliceTiming);

			// audioSourceSFX.PlayOneShot(soundEffect);

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

	void HandleSliceEffects(float sliceTiming)
	{
		if (sliceTiming <= GameManager.PERFECT_SWIPE_TIMING_INTERVAL.max && sliceTiming >= GameManager.PERFECT_SWIPE_TIMING_INTERVAL.min)
		{
			Destroy(Instantiate(particleEffectsPerfect, this.transform.position, Quaternion.identity), 3f);
			Destroy(Instantiate(fruitSlicedPrefab, this.transform.position, Quaternion.identity), 3f);
		}
		else if (sliceTiming < GameManager.GOOD_SWIPE_TIMING_INTERVAL.max && sliceTiming >= GameManager.GOOD_SWIPE_TIMING_INTERVAL.min)
		{
			Destroy(Instantiate(particleEffectsGood, this.transform.position, Quaternion.identity), 3f);
			Destroy(Instantiate(fruitSlicedPrefab, this.transform.position, Quaternion.identity), 3f);
		}
		else if (sliceTiming < GameManager.BAD_SWIPE_TIMING_INTERVAL.max && sliceTiming >= GameManager.BAD_SWIPE_TIMING_INTERVAL.min)
		{
			Destroy(Instantiate(particleEffectsBad, this.transform.position, Quaternion.identity), 3f);
			Destroy(Instantiate(fruitSlicedPrefab, this.transform.position, Quaternion.identity), 3f);
		}
		else if (sliceTiming > GameManager.PERFECT_SWIPE_TIMING_INTERVAL.max)
		{

			Destroy(Instantiate(particleEffectsBad, this.transform.position, Quaternion.identity), 3f);
			Destroy(Instantiate(fruitMissedSlicePrefab, this.transform.position, Quaternion.identity), 3f);
		}
		else if(sliceTiming < GameManager.BAD_SWIPE_TIMING_INTERVAL.max)
		{
			HandleSwipeMiss(sliceTiming);
			Destroy(Instantiate(particleEffectsMissed, this.transform.position, Quaternion.identity), 3f);
			Destroy(Instantiate(fruitMissedSlicePrefab, this.transform.position, Quaternion.identity), 3f);
		}

		//audioSourceSFX.PlayOneShot(soundEffect);
	}

	void HandleSwipeMiss(float sliceTiming)
	{
			ReduceHP();
	}

	void UpdateTimingCircleGraphics()
	{
		float timing = (audioSource.time - calibratedAnimationDelay) - beatTime;
		if(timing >= GameManager.BAD_SWIPE_TIMING_INTERVAL.min)
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
			GameManager.Instance.MissedSwipe();
			this.hasReducedHP = true;
		}
	}

	bool IsOutsideScreen()
	{
		Vector3 pos = mainCamera.WorldToScreenPoint(this.transform.position);
		bool outOfBounds = !Screen.safeArea.Contains(pos);
		return outOfBounds;
	}

	/* 360/8 = 45 grader
	 * 
	 * Om cylindern inte rör på sig kollar man swipe-input.
	 * När man swipear ett håll Slerpar cylindern m.h.a. https://docs.unity3d.com/ScriptReference/Quaternion.Slerp.html
	 * Så medan cylindern rör sig så är kanske en bool false. bool checkInput = false;
	 * 
	 * 
	 */

}
