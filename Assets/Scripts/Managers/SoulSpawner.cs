using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;
public class SoulSpawner : SingletonPattern<SoulSpawner>
{

    public enum SpawnPattern {CANNONBALLS, LINEAR, PAIR, TRIANGLE, SQUARE};
    GameObject objectToSpawn;
    SpawnPattern currentSpawnPattern;
    Vector2 lastTargetPosition;
    Bounds lastSquareBounds;
    MapSection fullMap = new MapSection 
    {
        upperLeftCorner = new Vector2(-7.5f, 2f),
        bottomRightCorner = new Vector2(7.5f, -2f)
    };
    MapSection leftMapSection = new MapSection
    {
        upperLeftCorner = new Vector2(-7.5f, 2f),
        bottomRightCorner = new Vector2(-2.5f, -2f)
    };
    MapSection middleMapSection = new MapSection 
    {
        upperLeftCorner = new Vector2(-2.5f, 2f),
        bottomRightCorner = new Vector2(2.5f, -2f)
    };
    MapSection rightMapSection = new MapSection 
    {
        upperLeftCorner = new Vector2(2.5f, 2f),
        bottomRightCorner = new Vector2(7.5f, -2f)
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
        currentSpawnPattern = SpawnPattern.CANNONBALLS;
        this.spawnPoints = spawnPoints.OrderBy(spawnPoint => spawnPoint.position.x).ToArray();
        lastSpawnPoint = this.spawnPoints[0];
    }

    public void SetBeatIndex(int beatIndex)
    {
        this.beatIndex = beatIndex;
    }

    public void SetSpawnPattern(SpawnPattern spawnPattern)
    {
        this.currentSpawnPattern = spawnPattern;
    }

    public int SpawnSoul(float spawnThresholdTime)
    {
        if(currentSpawnPattern == SpawnPattern.CANNONBALLS)
        {
            SpawnCannonBalls(spawnThresholdTime);
        } 
        else if(currentSpawnPattern == SpawnPattern.LINEAR)
        {
            SpawnLinear(spawnThresholdTime);
        } 
        else if (currentSpawnPattern == SpawnPattern.SQUARE) 
        {
            SpawnInSquare(spawnThresholdTime);
        } 
        else if (currentSpawnPattern == SpawnPattern.TRIANGLE) 
        {
            SpawnInTriangle(spawnThresholdTime);
        }
        else if (currentSpawnPattern == SpawnPattern.PAIR) 
        {
            SpawnInPair(spawnThresholdTime);
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
        // Vector2[] triangleTargets = GetSafeTriangleTargets();
        // SpawnInPattern(triangleTargets, spawnThresholdTime, SpawnPattern.TRIANGLE);
        Transform spawnPoint = GetSpawnLocation();
        Vector2[] triangleTargets = GetSafeTriangleTargets();
        SpawnInPattern(spawnPoint, triangleTargets, spawnThresholdTime, SpawnPattern.TRIANGLE);
    }

    void SpawnInPair(float spawnThresholdTime)
    {
        Transform spawnPoint = GetSpawnLocation();
        Vector2[] pairTargets = GetSafePairTargets();
        SpawnInPattern(spawnPoint, pairTargets, spawnThresholdTime, SpawnPattern.PAIR);
    }

    void SpawnInPattern(Vector2[] targets, float spawnThresholdTime, SpawnPattern spawnPattern)
    {
        Transform spawnPoint = GetSpawnLocation();
        foreach(Vector2 target in targets)
        { 
            SpawnPatternObject(spawnPoint, target, spawnThresholdTime, spawnPattern);
            
            beatIndex++;
            spawnPoint = GetSpawnLocation();
            if(beatIndex > SongHandler.Instance.GetAudioClipBeats().Count - 1)
            {
                break;
            }
        }
    }

    void SpawnInPattern(Transform spawnPoint, Vector2[] targets, float spawnThresholdTime, SpawnPattern spawnPattern)
    {
        foreach(Vector2 target in targets)
        {
            SpawnPatternObject(spawnPoint, target, spawnThresholdTime, spawnPattern);
            
            beatIndex++;
            if(beatIndex > SongHandler.Instance.GetAudioClipBeats().Count - 1)
            {
                break;
            }
        }
    }

    void SpawnPatternObject(Transform spawnPoint, Vector2 target, float spawnThresholdTime, SpawnPattern spawnPattern){
        GameObject spawnedFruit = Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);
        float beat = SongHandler.Instance.GetAudioClipBeats()[beatIndex];
        float timeDifference = beat - audioSourceMusic.time;
        Vector2 velocity = CalculateVelocity(target, spawnedFruit.transform.position, spawnThresholdTime - 0.2f);
        spawnedFruit.GetComponentInChildren<Fruit>().Initiate(audioSourceMusic, audioSourceSFX, beat, target, velocity, spawnPattern);
        Destroy(spawnedFruit, timeDifference + 5f);
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

    Bounds GetSafeBounds(Vector2? size)
    {
        Vector2 boundsSize = size.HasValue ? size.Value : new Vector2(3f, 2f);
        float randomX = UnityEngine.Random.Range(-7f, 7f);
        float randomY = UnityEngine.Random.Range(0.5f, -1.5f);
        Bounds squareBounds = new Bounds(new Vector2(randomX, randomY), boundsSize);
        
        while(squareBounds.Intersects(lastSquareBounds))
        {
            randomX = UnityEngine.Random.Range(-7f, 7f);
            randomY = UnityEngine.Random.Range(0.5f, -1.5f);
            squareBounds = new Bounds(new Vector2(randomX, randomY), boundsSize);
        }
        lastSquareBounds = squareBounds;
        return squareBounds;
    }

    Bounds GetSafeBoundsInMapSection(Vector2? size, MapSection mapSection)
    {
        Vector2 boundsSize = size.HasValue ? size.Value : new Vector2(3f, 2f);
        Vector2 position = AdjustPositionForPattern(GetSafeTargetPoint(mapSection));
        Bounds squareBounds = new Bounds(position, boundsSize);

        while(squareBounds.Intersects(lastSquareBounds))
        {
            position = GetSafeTargetPoint(mapSection);
            squareBounds = new Bounds(position, boundsSize);
        }
        lastSquareBounds = squareBounds;
        return squareBounds;
    }

    // To prevent the patterns from partly going off screen
    // their center position is adjusted if they are on the map edges
    Vector2 AdjustPositionForPattern(Vector2 position){
        float newX = position.x;
        float newY = position.y;
        float xAdjustment = 0.5f;
        float yAdjustment = 0.5f;

        // Adjust x position
        if(fullMap.bottomRightCorner.x == position.x){
            newX -= xAdjustment;
        }
        else if(fullMap.upperLeftCorner.x == position.x){
            newX += xAdjustment;
        }

        // Adjust y position
        if(fullMap.bottomRightCorner.y == position.y){
            newY += yAdjustment;
        }
        else if(fullMap.upperLeftCorner.y == position.y){
            newY -= yAdjustment;
        }
        return new Vector2(newX, newY);
    }

    Vector2[] GetSafePairTargets()
    {
        Bounds squareBounds = GetSafeBoundsInMapSection(new Vector2(1.5f, 2.25f), GetMapSection());
        float middleX = squareBounds.min.x + ((squareBounds.max.x - squareBounds.min.x) / 2f);
        return new Vector2[2] 
        {
            new Vector2(squareBounds.max.x - 0.25f, squareBounds.max.y),
            new Vector2(squareBounds.min.x + 0.25f, squareBounds.min.y)
        };
    }

    Vector2[] GetSafeSquareTargets()
    {
        Bounds squareBounds = GetSafeBounds(null);
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
        
        Bounds squareBounds = GetSafeBoundsInMapSection(new Vector2(3.5f, 2.2f), GetMapSection());
        float middleX = squareBounds.min.x + ((squareBounds.max.x - squareBounds.min.x) / 2f);
        return new Vector2[3] 
        {
            new Vector2(middleX, squareBounds.max.y - 0.1f),
            new Vector2(squareBounds.min.x + 0.25f, squareBounds.min.y + 0.1f),
            new Vector2(squareBounds.max.x - 0.25f, squareBounds.min.y + 0.1f)
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
