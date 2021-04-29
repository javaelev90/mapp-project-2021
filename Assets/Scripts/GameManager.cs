using System;
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
    [SerializeField] Transform[] spawnPoints;
    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource audioSourceSFX;
    [Header("UI")]
    [SerializeField] Button startGameButton;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] PopupScreenController popupScreenController;
    [SerializeField] Image hpBar;

    public static SwipeTimingInterval PERFECT_SWIPE_TIMING_INTERVAL = new SwipeTimingInterval(-0.2f, 0.2f);
    public static SwipeTimingInterval GOOD_SWIPE_TIMING_INTERVAL = new SwipeTimingInterval(0.2f, 0.4f);
    public static SwipeTimingInterval BAD_SWIPE_TIMING_INTERVAL = new SwipeTimingInterval(0.4f, 0.6f);


    public static float MAX_SWIPE_TIMING = 0.6f;
    public bool GameIsPaused { get; private set; } = false;

    //GameManager gameManager;
    SceneHandler sceneHandler;
    private int beatIndex = 0;
    private int spawnPointIndex = 0;
    private bool runGame = false;

    float hitPoints = 100f;
    float score = 0f;
    private bool wonGame;


    void Awake()
    {
        Initialize();
        sceneHandler = SceneHandler.Instance;
    }

    void Initialize()
    {
        GameManager.SetInstanceIfNull(this);
        startGameButton?.gameObject.SetActive(true);
    }

    void Update() 
    {
       if(runGame)
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
            runGame = true;
        }
    }

    public void RestartGame()
    {
        // Removing all Fruits in active scene
        foreach (var go in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (go.GetComponentInChildren<Fruit>() != null)
            {
                Destroy(go);
            }
        }

        // Reset game properties
        wonGame = false;
        runGame = false;
        beatIndex = 0;
        score = 0f;
        scoreText.text = "Score: " + score;
        SetHPBar(100f);
        hitPoints = 100f;
        audioSource.Stop();
        OnStartGame();
    }

    public void SetScore(float timing) //(vi f�r �ndra v�rdena sen )
    {
        if (timing > -.2f && timing < .2f) // Perfect
        {
            score += 100f;
        }
        else if (timing <= -.2f && timing > -.35f)  //Good, (�ndra v�rdena sen )
        {
            score += 50f;
        }
        else if (timing <= -.35f && timing > -.45f) // Bad, (�ndra v�rdena sen)
        {
            score += 25f;
        }
        else  // Miss
        {
            //scoreText.text = "Score: " + score;
        }
        scoreText.text = "Score: " + score;

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

    void SpawnSouls()
    {
        //check time difference
        //if difference smaller than num
        //spawn fruit
        //add countdown value
        //launch fruit
        //sleep for max prelaunch time value - current time in song

        float beat = SongHandler.Instance.GetAudioClipBeats()[beatIndex];
        float timeDifference = beat - audioSource.time;

        // Adding some randomness to spawn time
        float random = UnityEngine.Random.Range(-0.3f, 0.2f);

        // Spawn fruit if it is close enough to the next beat position in the music
        if (timeDifference <= SongHandler.Instance.GetPreferredMarginTimeBeforeBeat() + random)
        {
            // Rotate spawn position
            Transform spawnPoint = spawnPoints[spawnPointIndex % spawnPoints.Length];
            spawnPointIndex++;

            GameObject spawnedFruit = Instantiate(fruitPrefab, spawnPoint.position, spawnPoint.rotation);
            spawnedFruit.GetComponentInChildren<Fruit>().Initiate(audioSource, audioSourceSFX, beat);
            
            float verticalSpeed = UnityEngine.Random.Range(10f, 15f);
            float horizontalSpeed = 1f;
            
            // Setting horizontal speed depending on spawn point position
            // Left spawn
            if (spawnPointIndex % spawnPoints.Length == 0) {
                horizontalSpeed = UnityEngine.Random.Range(-4f, 1f);
            } 
            // Center spawn
            else if (spawnPointIndex % spawnPoints.Length == 1) {
                horizontalSpeed = UnityEngine.Random.Range(-1.5f, 1.5f);
            } 
            // Right spawn
            else {
                horizontalSpeed = UnityEngine.Random.Range(-1f, 4f);
            }
            
            // Adds upwards force to the fruit
            spawnedFruit.GetComponentInChildren<Rigidbody2D>().AddForce(transform.up * verticalSpeed, ForceMode2D.Impulse);
            spawnedFruit.GetComponentInChildren<Rigidbody2D>().AddForce(transform.right * horizontalSpeed, ForceMode2D.Impulse);
            
            beatIndex++;
            Destroy(spawnedFruit, 5f);
        }
        // The last beat has been spawned and the map is over
        if(beatIndex > SongHandler.Instance.GetAudioClipBeats().Count - 1)
        {
            runGame = false;
            wonGame = true;
        }
    }
    void WinGame()
    {
        if (!audioSource.isPlaying)
        {
            popupScreenController.ToggleWinScreen(true);
            int songScore = Database.Instance.songRepository.GetSongScore(SongHandler.Instance.GetSongName());
            if (songScore == -1 || songScore < score)
            {
                Database.Instance.songRepository.UpdateSongScore(SongHandler.Instance.GetSongName(), (int)score);

            }
            wonGame = false;
        }
    }

    void SetHPBar(float hitPoints)
    {
        hpBar.fillAmount = hitPoints / 100;
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