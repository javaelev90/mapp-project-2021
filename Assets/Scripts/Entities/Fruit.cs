using UnityEngine;

// TODO: Skapa object pooling f�r flygande objekt och partiklar
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Fruit : MonoBehaviour
{
	[Header("Game data")]
	[Tooltip("Spawned after slicing is successful")]
	[SerializeField] GameObject fruitSlicedPrefab;
	[Tooltip("Spawned after slicing is UNsuccessful")]
	[SerializeField] GameObject fruitMissedSlicePrefab;
	[SerializeField] Rigidbody2D myRigidbody;
	public Collider2D FruitCollider;
	[Header("Timing properties")]
	[SerializeField] SpriteRenderer timingCircle;
	[SerializeField] Color initialTimingCircleColor;
	[SerializeField] Color progressingTimingCircleColor;
	[SerializeField] Color finalTimingCircleColor;
	[Header("Animation")]
	[SerializeField] Animator ringAnimator;
	[SerializeField] AnimationClip ringAnimationClip;
	[Header("SFX")]
	[SerializeField] AudioClip[] breakGlassSFX;
	//[SerializeField] AudioClip soundEffect = null; // Används inte atm
	[Header("VFX")]
	[SerializeField] GameObject particleEffectsPerfect;
	[SerializeField] GameObject particleEffectsGood;
	[SerializeField] GameObject particleEffectsBad;
	[SerializeField] GameObject particleEffectsMissed;
	[SerializeField] GameObject particleDestroyFlash;


	bool hasReducedHP = false;
	float beatTime = 0f;
	AudioSource audioSourceMusic;
	AudioSource audioSourceSFX;
	Camera mainCamera;
	Vector3 target;
	float timeElapsed = 0f;

	float calibratedAnimationDelay = 0f;
	Vector2 startPosition;
	float timeToTargetPosition;
	bool hasReachedTarget = false;
	SoulSpawner.SpawnPattern spawnPattern;

	void Awake()
	{
		mainCamera = Camera.main;

		GameManager.Instance.Blade.OnFruitSliced += SliceMe;

		calibratedAnimationDelay = SongHandler.Instance.GetAudioLatency();
		timingCircle.color = new Color(initialTimingCircleColor.r, initialTimingCircleColor.b, initialTimingCircleColor.g, 0.2f);
	}

	void Update()
	{
		UpdateTimingCircleGraphics();
		HandleMovePattern();

		if (GetCurrentSliceTiming() > GameManager.PERFECT_SWIPE_TIMING_INTERVAL.max && IsOutsideScreen())
		{
			SoulSpawner.occupiedLocations.TryRemove(target, out _);
			ReduceHP();
		}
	}

	void OnDestroy()
	{	
		// Removes this object's reserved target on the map
		SoulSpawner.occupiedLocations.TryRemove(target, out _);
	}

	void OnDrawGizmos()
    {
#if UNITY_EDITOR
		// Draw the objects bounds square
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(target, FruitCollider.bounds.size);
#endif
    }

	public void Initiate(AudioSource audioSource, AudioSource audioSourceSFX, float beatTime, Vector3 target, Vector3 velocity, SoulSpawner.SpawnPattern spawnPattern)
	{
		this.target = target;
		this.timeToTargetPosition = beatTime - (audioSource.time - calibratedAnimationDelay);

		float countDown = Mathf.Abs(beatTime - audioSource.time);
		float animatorSpeed = 1 / ((countDown + calibratedAnimationDelay) / ringAnimationClip.length);
		this.audioSourceMusic = audioSource;
		this.beatTime = beatTime;
		this.audioSourceSFX = audioSourceSFX;
		this.spawnPattern = spawnPattern;
		this.startPosition = transform.position;

		ringAnimator.speed = animatorSpeed;
		ringAnimator.SetBool("ShrinkCircle", true);
		myRigidbody.velocity = velocity;

	}

	void SliceMe(Collider2D fruitCollider)
	{
		if (this.hasReducedHP || fruitCollider != FruitCollider)
			return;

		GameManager.Instance.SetScore(GetCurrentSliceTiming());
		HandleSliceEffects(GetCurrentSliceTiming());

		Destroy(this.gameObject);
	}

	void HandleMovePattern()
	{
		if (spawnPattern == SoulSpawner.SpawnPattern.LINEAR && !hasReachedTarget)
		{
			if (timeElapsed / timeToTargetPosition <= 1f)
			{
				transform.position = Vector3.Lerp(startPosition, target, timeElapsed / timeToTargetPosition);
				timeElapsed += Time.deltaTime;
			}
			else
			{
				hasReachedTarget = true;
				myRigidbody.velocity = Vector2.zero;
			}
		}
		else if (spawnPattern == SoulSpawner.SpawnPattern.TRIANGLE ||
			spawnPattern == SoulSpawner.SpawnPattern.PAIR ||
			spawnPattern == SoulSpawner.SpawnPattern.SQUARE)
		{
			if (Vector3.Distance(transform.position, target) <= 0.3f && GetCurrentSliceTiming() <= 0)
			{
				myRigidbody.bodyType = RigidbodyType2D.Static;
			}
			else
			{
				myRigidbody.bodyType = RigidbodyType2D.Dynamic;
			}
		}
	}

	void HandleSliceEffects(float sliceTiming)
	{
		if (sliceTiming <= GameManager.PERFECT_SWIPE_TIMING_INTERVAL.max && sliceTiming >= GameManager.PERFECT_SWIPE_TIMING_INTERVAL.min)
		{
			Destroy(Instantiate(particleEffectsPerfect, this.transform.position, Quaternion.identity), 3f);
			Destroy(Instantiate(fruitSlicedPrefab, this.transform.position, Quaternion.identity), 3f);
			PlayBrokenGlassEffect();
		}
		else if (sliceTiming < GameManager.GOOD_SWIPE_TIMING_INTERVAL.max && sliceTiming >= GameManager.GOOD_SWIPE_TIMING_INTERVAL.min)
		{
			Destroy(Instantiate(particleEffectsGood, this.transform.position, Quaternion.identity), 2f);
			Destroy(Instantiate(fruitSlicedPrefab, this.transform.position, Quaternion.identity), 3f);
			PlayBrokenGlassEffect();
		}
		else if (sliceTiming < GameManager.BAD_SWIPE_TIMING_INTERVAL.max && sliceTiming >= GameManager.BAD_SWIPE_TIMING_INTERVAL.min)
		{
			Destroy(Instantiate(particleEffectsBad, this.transform.position, Quaternion.identity), 2f);
			Destroy(Instantiate(fruitSlicedPrefab, this.transform.position, Quaternion.identity), 3f);
			PlayBrokenGlassEffect();
		}
		else if (sliceTiming > GameManager.PERFECT_SWIPE_TIMING_INTERVAL.max) // Bad post perfect
		{
			Destroy(Instantiate(particleEffectsBad, this.transform.position, Quaternion.identity), 2f);
			Destroy(Instantiate(fruitSlicedPrefab, this.transform.position, Quaternion.identity), 3f);
			PlayBrokenGlassEffect();
		}
		else if(sliceTiming < GameManager.BAD_SWIPE_TIMING_INTERVAL.max) // Miss
		{
			Destroy(Instantiate(particleEffectsMissed, this.transform.position, Quaternion.identity), 2f);
			Destroy(Instantiate(fruitMissedSlicePrefab, this.transform.position, Quaternion.identity), 3f);
			HandleSwipeMiss();
		}

	}
	void UpdateTimingCircleGraphics()
	{
		if (GetCurrentSliceTiming() >= GameManager.BAD_SWIPE_TIMING_INTERVAL.min
			&& timingCircle.color != progressingTimingCircleColor)
		{
			timingCircle.color = progressingTimingCircleColor;
		}
		if(GetCurrentSliceTiming() >= GameManager.PERFECT_SWIPE_TIMING_INTERVAL.min
			&& timingCircle.color != finalTimingCircleColor)
		{
			timingCircle.color = finalTimingCircleColor;
		}
		if (GetCurrentSliceTiming() > GameManager.PERFECT_SWIPE_TIMING_INTERVAL.max
			&& timingCircle.gameObject.activeSelf == true)
		{
			//timingCircle.enabled = false;
			timingCircle.gameObject.SetActive(false);
		}
	}

	void PlayBrokenGlassEffect()
	{
		int randomValue = (int)Random.Range(0f, breakGlassSFX.Length);
		audioSourceSFX.PlayOneShot(breakGlassSFX[randomValue]);
	}

	void HandleSwipeMiss() => ReduceHP();

	float GetCurrentSliceTiming() => audioSourceMusic.time - beatTime;

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

}
