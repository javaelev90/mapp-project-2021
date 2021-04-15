using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject fruitPrefab;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] AudioSource audioSource;

    void Start()
    {   
        if(SongHandler.Instance.GetSongAudioClip() != null)
        {
            StartCoroutine(SpawnFruits2());
        }
        else 
        {
            StartCoroutine(SpawnFruits());
        }
        
    }

    IEnumerator SpawnFruits()
    {
        while (true)
        {
            float delay = Random.Range(.1f, 1f);
            yield return new WaitForSeconds(delay);
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject spawnedFruit = Instantiate(fruitPrefab, spawnPoint.position, spawnPoint.rotation);
            spawnedFruit.GetComponent<Rigidbody2D>().AddForce(transform.up * Random.Range(10f, 15f), ForceMode2D.Impulse);
            spawnedFruit.GetComponent<Rigidbody2D>().AddForce(transform.right * Random.Range(-5f, 5f), ForceMode2D.Impulse);

            Destroy(spawnedFruit, 5f);
        }
    }
    IEnumerator SpawnFruits2()
    {
        List<float> beats = SongHandler.Instance.GetAudioClipBeats();
        float marginTime = SongHandler.Instance.GetPrefferdMarginTimeBeforeBeat();
        audioSource.clip = SongHandler.Instance.GetSongAudioClip();
        audioSource.Play();
        
        for(int i = 0; i < beats.Count; i++){
            
            //check time difference
            //if difference smaller than num
            //spawn fruit
            //add countdown value
            //launch fruit
            //sleep for max prelaunch time value - current time in song

            float beat = beats[i];
            float timeDifference = beat - audioSource.time;
            float spawnTimeDiff = Mathf.Abs(marginTime-timeDifference);
            if (timeDifference <= marginTime)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject spawnedFruit = Instantiate(fruitPrefab, spawnPoint.position, spawnPoint.rotation);
                spawnedFruit.GetComponent<Fruit>().Initiate(Mathf.Abs(beat - audioSource.time), audioSource, beat);
                spawnedFruit.GetComponent<Rigidbody2D>().AddForce(transform.up * Random.Range(10f, 15f), ForceMode2D.Impulse);
                spawnedFruit.GetComponent<Rigidbody2D>().AddForce(transform.right * Random.Range(-5f, 5f), ForceMode2D.Impulse);

                Destroy(spawnedFruit, 5f);
                yield return null;
            }
            else 
            {
                // Retry current beat after delay
                i--;
                yield return new WaitForSeconds(spawnTimeDiff);
            }
        }
    }
}
