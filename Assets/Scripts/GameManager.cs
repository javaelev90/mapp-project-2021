using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject fruitPrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] AudioSource audioSource;

    void Start()
    {   
        if(SongHandler.Instance.GetSongAudioClip() != null)
        {
            audioSource.clip = SongHandler.Instance.GetSongAudioClip();
            audioSource.Play();
        }
        StartCoroutine(SpawnFruits2());
    }

    IEnumerator SpawnFruits()
    {
        while (true)
        {
            float delay = Random.Range(.1f, 1f);
            yield return new WaitForSeconds(delay);
            
            GameObject spawnedFruit = Instantiate(fruitPrefab, spawnPoint.position, spawnPoint.rotation);
            spawnedFruit.GetComponent<Rigidbody2D>().AddForce(transform.up * Random.Range(10f, 15f), ForceMode2D.Impulse);
            spawnedFruit.GetComponent<Rigidbody2D>().AddForce(transform.right * Random.Range(-5f, 5f), ForceMode2D.Impulse);

            Destroy(spawnedFruit, 5f);
        }
    }
    IEnumerator SpawnFruits2()
    {
        //  while (true)
        // {
            List<float> beats = SongHandler.Instance.GetAudioClipBeats();
            float intervalMax = 2f;
            float intervalMin = 1f;
            for(int i = 0; i < beats.Count; i++){
                
                //check time difference
                //if difference smaller than num
                //spawn fruit
                //add countdown value
                //launch fruit
                //sleep for max prelaunch time value - current time in song

                float beat = beats[i];
                float timeDifference = beat - audioSource.time;
                float delay = Random.Range(intervalMin, intervalMax);
                float delayDiff = Mathf.Abs(delay-timeDifference);
                if (timeDifference <= delay)
                {
                    GameObject spawnedFruit = Instantiate(fruitPrefab, spawnPoint.position, spawnPoint.rotation);
                    spawnedFruit.GetComponent<Fruit>().Initiate(Mathf.Abs(beat - audioSource.time));
                    spawnedFruit.GetComponent<Rigidbody2D>().AddForce(transform.up * Random.Range(10f, 15f), ForceMode2D.Impulse);
                    spawnedFruit.GetComponent<Rigidbody2D>().AddForce(transform.right * Random.Range(-5f, 5f), ForceMode2D.Impulse);

                    Destroy(spawnedFruit, 5f);
                    yield return null;
                }
                else 
                {
                    // Redo current beat
                    i--;
                    yield return new WaitForSeconds(delayDiff);
                }
            }

        // }
    }
}
