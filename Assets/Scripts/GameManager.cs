using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject fruitPrefab;
    [SerializeField] Transform spawnPoint;

    void Start()
    {
        StartCoroutine(SpawnFruits());
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
}
