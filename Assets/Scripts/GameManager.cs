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

    private int beatIndex = 0;
    private int spawnPointIndex = 0;
    private bool runGame;

    void Start()
    {
        InitializeUI();
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
        // Might want to handle this differently in the future
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    /// <summary>
    /// Making sure the right UI is shown at start
    /// </summary>
    void InitializeUI()
    {
        startGameButton.gameObject.SetActive(true);
    }

}
