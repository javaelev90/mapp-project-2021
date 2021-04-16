using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject fruitPrefab;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Button startGameButton;

    private int beatIndex;
    private bool runGame;

    void Start()
    {   
        beatIndex = 0;
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
        if (timeDifference <= SongHandler.Instance.GetPrefferdMarginTimeBeforeBeat())
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject spawnedFruit = Instantiate(fruitPrefab, spawnPoint.position, spawnPoint.rotation);
            spawnedFruit.GetComponentInChildren<Fruit>().Initiate(audioSource, beat);
            spawnedFruit.GetComponentInChildren<Rigidbody2D>().AddForce(transform.up * Random.Range(10f, 15f), ForceMode2D.Impulse);
            spawnedFruit.GetComponentInChildren<Rigidbody2D>().AddForce(transform.right * Random.Range(-5f, 5f), ForceMode2D.Impulse);
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
}
