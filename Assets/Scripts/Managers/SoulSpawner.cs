using UnityEngine;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        upperLeftCorner = new Vector2(-7.5f, 2.5f),
        bottomRightCorner = new Vector2(7.5f, -2f)
    };
    public static MapSection leftMapSection = new MapSection
    {
        upperLeftCorner = new Vector2(-7.5f, 2.5f),
        bottomRightCorner = new Vector2(-2.5f, -2f)
    };
    public static MapSection middleMapSection = new MapSection 
    {
        upperLeftCorner = new Vector2(-2.5f, 2.5f),
        bottomRightCorner = new Vector2(2.5f, -2f)
    };
    public static MapSection rightMapSection = new MapSection 
    {
        upperLeftCorner = new Vector2(2.5f, 2.5f),
        bottomRightCorner = new Vector2(7.5f, -2f)
    };
    Transform[] spawnPoints;
    Transform lastSpawnPoint;
    AudioSource audioSourceMusic;
    AudioSource audioSourceSFX;

    public static ConcurrentDictionary<Vector2, Bounds> occupiedLocations;

    int beatIndex = 0;
    int spawnPointIndex = 0;

    int RETRY_LOOP_COUNT_LIMIT = 15;
    List<Vector2> locationsForMiddleMapSection;
    List<Vector2> locationsForRightMapSection;
    List<Vector2> locationsForLeftMapSection;

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
        occupiedLocations = new ConcurrentDictionary<Vector2, Bounds>();
        locationsForMiddleMapSection = FindLocations(middleMapSection,0.5f);
        locationsForRightMapSection = FindLocations(rightMapSection,0.5f);
        locationsForLeftMapSection = FindLocations(leftMapSection,0.5f);
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
        Vector2 target = GetSafeTargetPoint(GetMapSection(), new Vector2(2f, 2f));
        float beat = SongHandler.Instance.GetAudioClipBeats()[beatIndex];
        GameObject spawnedObject = Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);                
        Vector3 velocity = optionalVelocity.HasValue ? optionalVelocity.Value : CalculateVelocity(target, spawnedObject.transform.position, spawnThresholdTime);
        Destroy(spawnedObject, 5f);

        Fruit spawnedFruit = spawnedObject.GetComponentInChildren<Fruit>();
        spawnedFruit.Initiate(audioSourceMusic, audioSourceSFX, beat, target, velocity, spawnPattern);
        beatIndex++;  
        occupiedLocations[target] = new Bounds(
            target,
            new Vector2(
                spawnedFruit.FruitCollider.bounds.size.x,
                spawnedFruit.FruitCollider.bounds.size.y
            )
        );
    }

    void SpawnInSquare(float spawnThresholdTime)
    {
        Transform spawnPoint = GetSpawnLocation();
        Vector2[] squareTargets = GetSafeSquareTargets();
        SpawnInPattern(spawnPoint, squareTargets, spawnThresholdTime, SpawnPattern.SQUARE);
    }

    void SpawnInTriangle(float spawnThresholdTime)
    {
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

    void SpawnPatternObject(Transform spawnPoint, Vector2 target, float spawnThresholdTime, SpawnPattern spawnPattern)
    {
        GameObject spawnedObject = Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);
        float beat = SongHandler.Instance.GetAudioClipBeats()[beatIndex];
        float timeDifference = beat - audioSourceMusic.time;
        Vector2 velocity = CalculateVelocity(target, spawnedObject.transform.position, spawnThresholdTime - 0.2f);
        Destroy(spawnedObject, timeDifference + 5f);

        Fruit spawnedFruit = spawnedObject.GetComponentInChildren<Fruit>();
        spawnedFruit.Initiate(audioSourceMusic, audioSourceSFX, beat, target, velocity, spawnPattern);

        occupiedLocations[target] = new Bounds(
            target,
            new Vector2(
                spawnedFruit.FruitCollider.bounds.size.x,
                spawnedFruit.FruitCollider.bounds.size.y
            )
        );
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

    List<Vector2> GetMapSectionPositions()
    {
        if(spawnPointIndex == 0)
        {
            return locationsForLeftMapSection;
        } 
        else if (spawnPointIndex == 1)
        {
            return locationsForMiddleMapSection;
        } 
        else 
        {
            return locationsForRightMapSection;
        }
    }

    Transform GetSpawnLocation()
    {
        spawnPointIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnPointIndex];
        int breakCounter = 0;
        while(lastSpawnPoint.position == spawnPoint.position)
        {
            spawnPointIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
            spawnPoint = spawnPoints[spawnPointIndex];
            //Prevent game lockup, should not happen but just in case
            if(breakCounter > RETRY_LOOP_COUNT_LIMIT)
            {
                break;
            }
            breakCounter++;
        }
        lastSpawnPoint = spawnPoint;

        return spawnPoint;
    }

    Bounds GetSafeBoundsInMapSection(Vector2? size, MapSection mapSection)
    {
        Vector2 boundsSize = size.HasValue ? size.Value : new Vector2(2f, 2f);
        Vector2 position = AdjustPositionForPattern(GetSafeTargetPoint(mapSection, boundsSize));
        
        Bounds squareBounds = new Bounds(position, boundsSize);
        return squareBounds;
    }

    // To prevent the patterns from partly going off screen
    // their center position is adjusted if they are on the map edges
    Vector2 AdjustPositionForPattern(Vector2 position)
    {
        float newX = position.x;
        float newY = position.y;
        float xAdjustment = 0.75f;
        float yAdjustment = 0.75f;

        // Adjust x position
        if(Mathf.Abs(fullMap.bottomRightCorner.x - position.x) <= 0.5f)
        {
            newX -= xAdjustment;
        }
        else if(Mathf.Abs(fullMap.upperLeftCorner.x - position.x) <= 0.5f)
        {
            newX += xAdjustment;
        }

        // Adjust y position
        if(Mathf.Abs(fullMap.bottomRightCorner.y - position.y) <= 0.5f)
        {
            newY += yAdjustment;
        }
        else if(Mathf.Abs(fullMap.upperLeftCorner.y - position.y) <= 0.5f)
        {
            newY -= yAdjustment;
        }
        return new Vector2(newX, newY);
    }

    Vector2[] GetSafePairTargets()
    {
        Bounds squareBounds = GetSafeBoundsInMapSection(new Vector2(1f, 2f), GetMapSection());
        float middleX = squareBounds.min.x + ((squareBounds.max.x - squareBounds.min.x) / 2f);
        return new Vector2[2] 
        {
            new Vector2(squareBounds.max.x, squareBounds.max.y),
            new Vector2(squareBounds.min.x, squareBounds.min.y)
        };
    }

    Vector2[] GetSafeSquareTargets()
    {
        Bounds squareBounds = GetSafeBoundsInMapSection(new Vector2(3f, 2f), GetMapSection());
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
        Bounds squareBounds = GetSafeBoundsInMapSection(new Vector2(3f, 2f), GetMapSection());
        float middleX = squareBounds.min.x + ((squareBounds.max.x - squareBounds.min.x) / 2f);
        return new Vector2[3] 
        {
            new Vector2(middleX, squareBounds.max.y),
            new Vector2(squareBounds.min.x, squareBounds.min.y),
            new Vector2(squareBounds.max.x, squareBounds.min.y)
        };
    }
    
    Vector2 GetTargetPoint(MapSection mapSection, Vector2 boundsSize)
    {
        float stepSize = 0.5f;
        float randomX = UnityEngine.Random.Range(mapSection.upperLeftCorner.x, mapSection.bottomRightCorner.x);
        float randomY = UnityEngine.Random.Range(mapSection.upperLeftCorner.y, mapSection.bottomRightCorner.y);
        float stepsX = Mathf.Floor(Mathf.Abs(randomX / stepSize));
        float stepsY = Mathf.Floor(Mathf.Abs(randomY / stepSize));
        
        float spawnPositionX = Mathf.Sign(randomX) * stepsX * stepSize;
        float spawnPositionY = Mathf.Sign(randomY) * stepsY * stepSize;
        Vector2 availableRandomLocation = new Vector2(spawnPositionX, spawnPositionY);
        
        List<Vector2> availableLocations = FindAvailableLocations(GetMapSectionPositions(), boundsSize);
        if(availableLocations.Count() > 0)
        {
            int randomLocation = UnityEngine.Random.Range(0, availableLocations.Count());
            return availableLocations[randomLocation];
        }
        else
        {   
            Debug.LogWarning("No available location, supplying random location instead.");
            return availableRandomLocation;    
        }
    }

    List<Vector2> FindLocations(MapSection mapSection, float stepSize)
    {
        List<Vector2> possibleLocations = new List<Vector2>();
        for(float x = mapSection.upperLeftCorner.x; x <= mapSection.bottomRightCorner.x; x += stepSize)
        {
            for(float y = mapSection.upperLeftCorner.y; y >= mapSection.bottomRightCorner.y; y -= stepSize)
            {
                Vector2 location = new Vector2(x, y);
    
                if(!occupiedLocations.ContainsKey(location))
                {
                    possibleLocations.Add(location);
                } 
            }
        }
        
        return possibleLocations;
    }

    List<Vector2> FindAvailableLocations(List<Vector2> possibleLocations, Vector2 boundsSize)
    {
        List<Vector2> availableLocations = new List<Vector2>();

        foreach(Vector2 location in possibleLocations)
        {
            if(!IsCollidingWithSoul(location, boundsSize))
            {
                availableLocations.Add(location);
            }
        }
        return availableLocations;
    }

    Vector2 GetSafeTargetPoint(MapSection mapSection, Vector2 boundsSize)
    {
        return GetTargetPoint(mapSection, boundsSize);
    }

    bool IsCollidingWithSoul(Vector2 target, Vector2 boundsSize)
    {
        Bounds targetBounds = new Bounds(target, boundsSize);
        Bounds locationBounds;

        foreach (Vector2 location in occupiedLocations.Keys)
        {
            occupiedLocations.TryGetValue(location, out locationBounds);
            if(locationBounds != null && locationBounds.Intersects(targetBounds))
            {
                return true;
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
