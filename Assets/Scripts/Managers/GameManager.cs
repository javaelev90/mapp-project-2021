using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : SingletonPattern<GameManager>
{
    [Header("Gameplay")]
    [SerializeField][Range(1, 25)] int pointsLostOnMiss = 20;
    [Header("Game Data")]
    [SerializeField] GameObject fruitPrefab;
    [SerializeField] GameObject spiritsMoveTarget;
    [Tooltip("The touchBlade prefab in the game scene")]
    [SerializeField] Blade blade;
    [SerializeField] Transform[] spawnPoints;
    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource audioSourceSFX;
    [Header("UI")]
    [SerializeField] Button startGameButton;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] PopupScreenController popupScreenController;
    [SerializeField] GameObject hpBarFiller;
    [SerializeField] GameObject hpBarFillerSpacer;
    [SerializeField] PlayFabManager playFabManager;
    [SerializeField] ParticleSystem winParticleEffect;

    public static SwipeTimingInterval PERFECT_SWIPE_TIMING_INTERVAL = new SwipeTimingInterval(-.2f, .2f);
    public static SwipeTimingInterval GOOD_SWIPE_TIMING_INTERVAL = new SwipeTimingInterval(-.4f, -.2f);
    public static SwipeTimingInterval BAD_SWIPE_TIMING_INTERVAL = new SwipeTimingInterval(-.6f, -.4f);

    public static float MAX_SWIPE_TIMING = 0.6f;
    public bool GameIsPaused { get; private set; } = false;
    public bool RunningGame { get; private set; } = false;
    public GameObject SpiritsMoveTarget => spiritsMoveTarget;
    public Blade Blade => blade;

    [System.NonSerialized] public int beatIndex = 0;

    Image hpBarImage;
    Animator hpBarAnimator;
    float hitPoints = 100f;
    float calibratedAnimationDelay = 0f;
    float score = 0f;

    bool wonGame;
    SoulSpawner spawner;
    [SerializeField] MenuPageChangeController screenOverlayFader;

    void Awake()
    {
        Initialize();
        SetTargetFPS();
        if(screenOverlayFader != null) screenOverlayFader.FadeInScene();
    }

    void Initialize()
    {
        UnityEngine.Random.InitState(42); // The answer to everything?
        GameManager.SetInstanceIfNull(this);
        startGameButton?.gameObject.SetActive(true);
        spawner = SoulSpawner.Instance;
        spawner.Initialize(spawnPoints, beatIndex, fruitPrefab, audioSource, audioSourceSFX);
        calibratedAnimationDelay = SongHandler.Instance.GetAudioLatency();

        hpBarImage = hpBarFiller?.GetComponentInChildren<Image>();
        hpBarAnimator = hpBarFiller?.GetComponentInChildren<Animator>();
        InitializeHpBar();
    }

    void SetTargetFPS()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop)
            Application.targetFrameRate = 120;
        else if (Database.Instance.settingsRepository.GetFPSSetting() == 1f)
            Application.targetFrameRate = -1;
        else if (Database.Instance.settingsRepository.GetFPSSetting() == 0f ||
            Database.Instance.settingsRepository.GetFPSSetting() == 999f)
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
    }

    void InitializeHpBar()
    {
        int numberOfSpacers = (int) hitPoints / pointsLostOnMiss;
        float spacePosition = hpBarFiller.GetComponent<RectTransform>().rect.width / numberOfSpacers;
        float leftMostLocalPosition = 0f - hpBarFiller.GetComponent<RectTransform>().rect.width/2f;
        for(int i = 0; i < numberOfSpacers; i++)
        {
            GameObject spacer = Instantiate(hpBarFillerSpacer, hpBarFiller.transform.position, hpBarFiller.transform.rotation);
            spacer.transform.SetParent(hpBarFiller.transform);
            spacer.transform.localScale = Vector3.one;
            spacer.transform.localPosition = new Vector2(leftMostLocalPosition, 0);
            leftMostLocalPosition += spacePosition;
        }
    }
    void OnDrawGizmos()
    {
#if UNITY_EDITOR
		Gizmos.color = Color.red;
		Gizmos.DrawLine(SoulSpawner.leftMapSection.bottomRightCorner, SoulSpawner.middleMapSection.upperLeftCorner);
        Gizmos.DrawLine(SoulSpawner.middleMapSection.bottomRightCorner, SoulSpawner.rightMapSection.upperLeftCorner);
#endif
    }

    void Update() 
    {
       if(RunningGame)
       {
           SpawnSouls();
       }

       if(wonGame)
        {
            WinGame();
        }
    }

    public void OnStartGame()
    {
        if (SongHandler.Instance.GetSongAudioClip() != null)
        {
            startGameButton.gameObject.SetActive(false);
            audioSource.clip = SongHandler.Instance.GetSongAudioClip();
            audioSource.Play();
            TogglePause(false);
            RunningGame = true;
        }
    }

    public void RestartGame()
    {
        audioSource.time = 0;
        audioSource.Stop();
        
        if(winParticleEffect != null) 
        {
            winParticleEffect.Stop();
            winParticleEffect.Clear();
        }

        // Removing objects in active scene
        foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (go.GetComponentInChildren<Fruit>() != null || go.GetComponentInChildren<SlicedFruit>() != null)
            {
                Destroy(go);
            }
        }

        // Reset game properties
        wonGame = false;
        RunningGame = false;
        beatIndex = 0;
        spawner.SetBeatIndex(beatIndex);
        score = 0f;
        scoreText.text = "";
        SetHPBar(100f);
        hitPoints = 100f;
        OnStartGame();
        
        // PlayerPrefs are used for song editing functionality
#if UNITY_EDITOR
        PlayerPrefs.SetInt("RestartedGame",1);
#endif
    }

    public void SetScore(float timing)
    {
        if (timing > PERFECT_SWIPE_TIMING_INTERVAL.min && timing < PERFECT_SWIPE_TIMING_INTERVAL.max)
        {
            score += 100f;
        }
        else if (timing <= GOOD_SWIPE_TIMING_INTERVAL.max && timing > GOOD_SWIPE_TIMING_INTERVAL.min)
        {
            score += 50f;
        }
        else if (timing <= BAD_SWIPE_TIMING_INTERVAL.max && timing > BAD_SWIPE_TIMING_INTERVAL.min)
        {
            score += 25f;
        }

        scoreText.text = score.ToString();
    }

    public void MissedSwipe()
    {
        hitPoints -= pointsLostOnMiss;
        if (hitPoints <= 0f)
        {
            SetHPBar(0f);
            GameOver();
        }
        else
            SetHPBar(hitPoints);
    }

    public void TogglePause(bool pause)
    {
        GameIsPaused = pause;
        if (GameIsPaused)
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }
        else
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    }
    
    public void SetBeatIndex(int beatIndex)
    {   
        this.beatIndex = beatIndex;
        this.spawner.SetBeatIndex(beatIndex);
    }

    void SpawnSouls()
    {
        //check time difference
        //if difference smaller than num
        //spawn fruit
        //add countdown value
        //launch fruit

        float beat = SongHandler.Instance.GetAudioClipBeats()[beatIndex];
        spawner.SetSpawnPattern(GetSpawnPattern(beat));

        float timeDifference = beat - audioSource.time;

        // Adding some randomness to spawn time
        float random = UnityEngine.Random.Range(-0.1f, 0.3f);

        // Spawn fruit if it is close enough to the next beat position in the music
        float spawnThresholdTime = SongHandler.Instance.GetPreferredMarginTimeBeforeBeat() + random;

        if (timeDifference <= spawnThresholdTime)
        {
            beatIndex = spawner.SpawnSoul(spawnThresholdTime);
        }
        // The last beat has been spawned and the map is over
        if(beatIndex > SongHandler.Instance.GetAudioClipBeats().Count - 1)
        {
            RunningGame = false;
            wonGame = true;
        }
    }

    SoulSpawner.SpawnPattern GetSpawnPattern(float currentMusicTime)
    {
        SoulSpawner.SpawnPattern spawnPattern = SoulSpawner.SpawnPattern.CANNONBALLS;
        
        foreach(SpawnPatternChange spawnPatternChange in SongHandler.Instance.GetSpawnChangePatterns())
        {
            if(currentMusicTime >= spawnPatternChange.activationTime)
            {
                spawnPattern = spawnPatternChange.spawnPattern;
            }
        }
        return spawnPattern;
    }

    void WinGame()
    {
        if (!audioSource.isPlaying)
        {
            popupScreenController.ToggleWinScreen(true);
            int songScore = Database.Instance.songRepository.GetSongScore(SongHandler.Instance.GetUniqueSongName());
            if (songScore == -1 || songScore < score)
            {
                Database.Instance.songRepository.UpdateSongScore(SongHandler.Instance.GetUniqueSongName(), (int)score);
                
            }
            // Always send score to leaderboard incase of internet problems.
            // Leaderboard setting will save the highest score sent from the user.
            if(songScore > score)
            {
                if(playFabManager != null) playFabManager.SendLeaderboard(songScore, SongHandler.Instance.GetUniqueSongName());
            }
            else
            {
                if(playFabManager != null) playFabManager.SendLeaderboard((int)score, SongHandler.Instance.GetUniqueSongName());
            }
            if(winParticleEffect != null) winParticleEffect.Play();
            wonGame = false;
        }
    }

    void SetHPBar(float hitPoints)
    {
        hpBarImage.fillAmount = hitPoints / 100;
        hpBarAnimator.Play("ChangeEffect");
    }

    void GameOver()
    {
        TogglePause(true);
        popupScreenController.ToggleLooseScreen(true);
    }

}

public class SwipeTimingInterval
{
    public SwipeTimingInterval(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
    public float min;
    public float max;
 
}