using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;
public class SoulSpawner : SingletonPattern<SoulSpawner>
{

    public enum SpawnPattern {CANNONBALLS, LINEAR, TRIANGLE, SQUARE};
    GameObject objectToSpawn;
    SpawnPattern currentSpawnPhase;
    Vector2 lastTargetPosition;
    Bounds lastSquareBounds;
    MapSection fullMap = new MapSection 
    {
        upperLeftCorner = new Vector2(-8.0f, 3f),
        bottomRightCorner = new Vector2(8.0f, -2f)
    };
    MapSection leftMapSection = new MapSection
    {
        upperLeftCorner = new Vector2(-8f, 3f),
        bottomRightCorner = new Vector2(-2.5f, -2f)
    };
    MapSection middleMapSection = new MapSection 
    {
        upperLeftCorner = new Vector2(-2.5f, 3f),
        bottomRightCorner = new Vector2(2.5f, -2f)
    };
    MapSection rightMapSection = new MapSection 
    {
        upperLeftCorner = new Vector2(2.5f, 3f),
        bottomRightCorner = new Vector2(8f, -2f)
    };
    Transform[] spawnPoints;
    Transform lastSpawnPoint;
    AudioSource audioSourceMusic;
    AudioSource audioSourceSFX;

    int beatIndex = 0;
    int spawnPointIndex = 0;

    public void Initialize(Transform[] spawnPoints, int beatIndex, GameObject objectToSpawn, AudioSource music, AudioSource sfx)
    {   
        this.spawnPoints = spawnPoints;
        this.audioSourceMusic = music;
        this.audioSourceSFX = sfx;
        this.objectToSpawn = objectToSpawn;
        this.beatIndex = beatIndex;
        currentSpawnPhase = SpawnPattern.CANNONBALLS;
        this.spawnPoints = spawnPoints.OrderBy(spawnPoint => spawnPoint.position.x).ToArray();
        lastSpawnPoint = this.spawnPoints[0];
    }

    public void SetBeatIndex(int beatIndex)
    {
        this.beatIndex = beatIndex;
    }

    public void SetSpawnPattern(SpawnPattern spawnPattern)
    {
        this.currentSpawnPhase = spawnPattern;
    }

    public int SpawnSoul(float spawnThresholdTime)
    {
        if(currentSpawnPhase == SpawnPattern.CANNONBALLS)
        {
            SpawnCannonBalls(spawnThresholdTime);
        } 
        else if(currentSpawnPhase == SpawnPattern.LINEAR)
        {
            SpawnLinear(spawnThresholdTime);
        } 
        else if (currentSpawnPhase == SpawnPattern.SQUARE) 
        {
            SpawnInSquare(spawnThresholdTime);
        } 
        else if (currentSpawnPhase == SpawnPattern.TRIANGLE) 
        {
            SpawnInTriangle(spawnThresholdTime);
        }
        return beatIndex;
    }

    void SpawnCannonBalls(float spawnThresholdTime)
    {
        SpawnSingle(spawnThresholdTime, null, SpawnPattern.CANNONBALLS);
     
    }

    void SpawnLinear(float spawnThresholdTime)
    {
        SpawnSingle(spawnThresholdTime, Vector3.zero, SpawnPattern.LINEAR);
    }

    void SpawnSingle(float spawnThresholdTime, Vector3? optionalVelocity, SpawnPattern spawnPattern)
    {
        Transform spawnPoint = GetSpawnLocation();
        Vector2 target = GetSafeTargetPoint(GetMapSection());
        float beat = SongHandler.Instance.GetAudioClipBeats()[beatIndex];
        GameObject spawnedFruit = Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);                
        Vector3 velocity = optionalVelocity.HasValue ? optionalVelocity.Value : CalculateVelocity(target, spawnedFruit.transform.position, spawnThresholdTime);

        spawnedFruit.GetComponentInChildren<Fruit>().Initiate(audioSourceMusic, audioSourceSFX, beat, target, velocity, spawnPattern);
        Destroy(spawnedFruit, 5f);    
        beatIndex++;  
    }

    void SpawnInSquare(float spawnThresholdTime)
    {
        Vector2[] squareTargets = GetSafeSquareTargets();
        SpawnInPattern(squareTargets, spawnThresholdTime, SpawnPattern.SQUARE);
    }

    void SpawnInTriangle(float spawnThresholdTime)
    {
        Vector2[] triangleTargets = GetSafeTriangleTargets();
        SpawnInPattern(triangleTargets, spawnThresholdTime, SpawnPattern.TRIANGLE);
    }

    void SpawnInPattern(Vector2[] targets, float spawnThresholdTime, SpawnPattern spawnPattern)
    {
        Transform spawnPoint = GetSpawnLocation();
        foreach(Vector2 target in targets)
        {
            GameObject spawnedFruit = Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);
            float beat = SongHandler.Instance.GetAudioClipBeats()[beatIndex];
            float timeDifference = beat - audioSourceMusic.time;
            Vector2 velocity = CalculateVelocity(target, spawnedFruit.transform.position, spawnThresholdTime - 0.2f);
            spawnedFruit.GetComponentInChildren<Fruit>().Initiate(audioSourceMusic, audioSourceSFX, beat, target, velocity, spawnPattern);
            
            beatIndex++;
            Destroy(spawnedFruit, timeDifference + 5f);

            spawnPoint = GetSpawnLocation();
            if(beatIndex > SongHandler.Instance.GetAudioClipBeats().Count - 1)
            {
                break;
            }
        }
    }

    MapSection GetMapSection()
    {
        if(spawnPointIndex == 0)
        {
            return leftMapSection;
        } 
        else if (spawnPointIndex == 1)
        {
            return middleMapSection;
        } 
        else 
        {
            return rightMapSection;
        }
    }

    Transform GetSpawnLocation()
    {
        spawnPointIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnPointIndex];
        while(lastSpawnPoint.position == spawnPoint.position)
        {
            spawnPointIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
            spawnPoint = spawnPoints[spawnPointIndex];
        }
        lastSpawnPoint = spawnPoint;

        return spawnPoint;
    }

    Bounds GetSafeBounds()
    {
        float randomX = UnityEngine.Random.Range(-3, 7);
        float randomY = UnityEngine.Random.Range(0, -2);
        Bounds squareBounds = new Bounds(new Vector2(randomX, randomY), new Vector2(4f, 3f));
        while(squareBounds.Intersects(lastSquareBounds))
        {
            randomX = UnityEngine.Random.Range(-3, 7);
            randomY = UnityEngine.Random.Range(0, -2);
            squareBounds = new Bounds(new Vector2(randomX, randomY), new Vector2(4f, 3f));
        }
        lastSquareBounds = squareBounds;
        return squareBounds;
    }
    Vector2[] GetSafeSquareTargets()
    {
        Bounds squareBounds = GetSafeBounds();
        return new Vector2[4] 
        {
            new Vector2(squareBounds.min.x, squareBounds.min.y),
            new Vector2(squareBounds.min.x, squareBounds.max.y),
            new Vector2(squareBounds.max.x, squareBounds.min.y),
            new Vector2(squareBounds.max.x, squareBounds.max.y)
        };
    }

    Vector2[] GetSafeTriangleTargets()
    {
        Bounds squareBounds = GetSafeBounds();
        float middleX = squareBounds.min.x + ((squareBounds.max.x - squareBounds.min.x) / 2f);
        return new Vector2[3] 
        {
            new Vector2(middleX, squareBounds.max.y),
            new Vector2(squareBounds.min.x, squareBounds.min.y),
            new Vector2(squareBounds.max.x, squareBounds.min.y)
        };
    }
    Vector2 GetTargetPoint(MapSection mapSection){
        
        float stepSize = 0.5f;
        float randomX = UnityEngine.Random.Range(mapSection.upperLeftCorner.x, mapSection.bottomRightCorner.x);
        float randomY = UnityEngine.Random.Range(mapSection.upperLeftCorner.y, mapSection.bottomRightCorner.y);
        float stepsX = Mathf.Floor(Mathf.Abs(randomX / stepSize));
        float stepsY = Mathf.Floor(Mathf.Abs(randomY / stepSize));
        
        float spawnPositionX = Mathf.Sign(randomX) * stepsX * stepSize;
        float spawnPositionY = Mathf.Sign(randomY) * stepsY * stepSize;

        return new Vector2(spawnPositionX, spawnPositionY);        
    }

    Vector2 GetSafeTargetPoint(MapSection mapSection)
    {
        Vector2 targetPosition = GetTargetPoint(mapSection);
        // while(targetPosition.Equals(lastPosition) || squareBounds.Contains(targetPosition))
        while(targetPosition.Equals(lastTargetPosition) || IsCollidingWithSoul(targetPosition) || IsTargetInBounds(targetPosition))
        {
            targetPosition = GetTargetPoint(mapSection);
        }
        lastTargetPosition = targetPosition;
        return targetPosition;
    }

    bool IsCollidingWithSoul(Vector2 target)
    {
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Soul");
        foreach(GameObject soul in allObjects)
        {
            if(soul != null)
            {
                if(soul.GetComponent<Collider2D>().bounds.Contains(target))
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool IsTargetInBounds(Vector2 target)
    {
        if(lastSquareBounds.size == Vector3.zero)
        {
            return false;
        } 
        else
        {
            bool inBounds = lastSquareBounds.Contains(target);
            lastSquareBounds = new Bounds();
            return inBounds;
        }
    }

    Vector3 CalculateVelocity(Vector3 target, Vector3 currentPosition,float time)
    {
        float vx = (target.x -currentPosition.x) / time;
        float vy = ((target.y -currentPosition.y) - 0.5f * Physics.gravity.y * time * time) / time;
        return new Vector3(vx, vy, 0f);
    }

}

public class MapSection
{
    public Vector2 upperLeftCorner;
    public Vector2 bottomRightCorner;
}
