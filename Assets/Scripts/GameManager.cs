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
    [SerializeField] GameObject winScreen;
    [SerializeField] Image hpBar;

    //GameManager gameManager;
    SceneHandler sceneHandler;
    private int beatIndex = 0;
    private int spawnPointIndex = 0;
    private bool runGame = false;

    float hitPoints = 100;
    float score = 0f;
    private bool wonGame;

    void Awake()
    {
        InitializeUI();
        sceneHandler = SceneHandler.Instance;
        GameManager.SetInstanceIfNull(this);
    }

    /// <summary>
    /// Making sure the right UI is shown
    /// </summary>
    void InitializeUI()
    {
        startGameButton?.gameObject.SetActive(true);

    }

    private void Update() 
    {
       if(runGame)
       {
           SpawnSouls();
       }

       if(wonGame) 
       {
           if(!audioSource.isPlaying) 
           {
               winScreen.SetActive(true);
               int songScore = Database.Instance.songRepository.GetSongScore(SongHandler.Instance.GetSongName());
               if(songScore == -1 || songScore < score)
               {
                   Database.Instance.songRepository.UpdateSongScore(SongHandler.Instance.GetSongName(), (int) score);
               
               }
               wonGame = false;
           }
       }
    }

    private void SpawnSouls()
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

    public void OnStartGame()
    {
        if (SongHandler.Instance.GetSongAudioClip() != null)
        {
            startGameButton.gameObject.SetActive(false);
            audioSource.clip = SongHandler.Instance.GetSongAudioClip();
            audioSource.Play();
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
        score = 0;
        scoreText.text = "Score: " + score;
        audioSource.Stop();
        OnStartGame();
    }

    public void SetScore(float timing) //(vi f�r �ndra v�rdena sen )
    {
        if (timing < .2f && timing > -.2f) { // Perfect
            
            score += 100;
            scoreText.text = "Score: " + score; 

        } else if (timing > .2f && timing < .4f)  //Good, (�ndra v�rdena sen )
        {   
            score += 50;
            scoreText.text = "Score: " + score; 

        } else if (timing > .4f && timing < .6f) // Bad, (�ndra v�rdena sen)
        {
            score += 25;
            scoreText.text = "Score: " + score;

        } else  // Miss
        {
            
            scoreText.text = "Score: " + score;

            
        }
        

    }

    public void MissedSwipe()
    {
        hitPoints -= pointsLostOnMiss;
        if (hitPoints < 0)
            GameOver();
        else
            SetHPBar(hitPoints);
    }

    void SetHPBar(float hitPoints)
    {
        hpBar.fillAmount = hitPoints / 100;
    }

    void GameOver()
    {

    }
}
