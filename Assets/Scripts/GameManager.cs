using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : SingletonPattern<GameManager>
{
    [Header("Gameplay")]
    [SerializeField] GameObject fruitPrefab;
    [SerializeField] Transform[] spawnPoints;
    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource audioSourceSFX;
    [Header("UI")]
    [SerializeField] Button startGameButton;
    [SerializeField] TMP_Text scoreText;

    SceneHandler sceneHandler;
    private int beatIndex = 0;
    private int spawnPointIndex = 0;
    private bool runGame = false;

    int hitPoints = 0;
    float score = 0;

    void Awake()
    {
        InitializeUI();
        sceneHandler = SceneHandler.Instance;
    }

    private void Update() 
    {
       if(runGame)
       {
           SpawnSouls();
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
        if (timeDifference <= SongHandler.Instance.GetPreferredMarginTimeBeforeBeat())
        {
            Transform spawnPoint = spawnPoints[spawnPointIndex % spawnPoints.Length];
            spawnPointIndex++;
            GameObject spawnedFruit = Instantiate(fruitPrefab, spawnPoint.position, spawnPoint.rotation);
            spawnedFruit.GetComponentInChildren<Fruit>().Initiate(audioSource, audioSourceSFX, beat);
            spawnedFruit.GetComponentInChildren<Rigidbody2D>().AddForce(transform.up * UnityEngine.Random.Range(10f, 15f), ForceMode2D.Impulse);
            spawnedFruit.GetComponentInChildren<Rigidbody2D>().AddForce(transform.right * UnityEngine.Random.Range(-5f, 5f), ForceMode2D.Impulse);
            beatIndex++;
            Destroy(spawnedFruit, 5f);
        }
        if(beatIndex > SongHandler.Instance.GetAudioClipBeats().Count - 1)
        {
            runGame = false;
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

        // Somehow restart
    }

    /// <summary>
    /// Making sure the right UI is shown
    /// </summary>
    void InitializeUI()
    {
        startGameButton?.gameObject.SetActive(true);
    }

    public void SetScore(float timing)
    {
        if (timing < .2f && timing > -.2f) { // Perfect
            score = 100;
            scoreText.text = "Score: " + score;
        }
        //else if ()
    }
}
