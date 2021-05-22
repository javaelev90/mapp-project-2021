using UnityEngine;

/// <summary>
/// This class is an example of how to detect swipes.
/// </summary>
public class SwipeDetection : MonoBehaviour
{
    [Tooltip("Min distance for a touch input to be counted as a swipe")]
    [SerializeField] float minimumDistance = .2f;
    [Tooltip("Max time for a touch input to be counted as a swipe")]
    [SerializeField] float maximumTime = 1f;
    [Tooltip("The sensitivity of the detection of directional swipes, 1 = swipe must align 100% to one direction")]
    [SerializeField, Range(0f, 1f)] float directionThreshold = .9f;

    InputManager inputManager;

    // Variables for calculating swipes
    Vector2 startPosition;
    float startTime;
    Vector2 endPosition;
    float endTime;

    void Awake()
    {
        inputManager = InputManager.Instance;
    }

    void OnEnable()
    {
        // Events
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;
    }
    void OnDisable()
    {
        // Events
        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
    }

    void SwipeStart(Vector2 position, float time)
    {
        startPosition = position;
        //print("touch start pos: " + startPosition);

        startTime = time;
    }
    void SwipeEnd(Vector2 position, float time)
    {
        endPosition = position;
        //print("touch end pos: " + endPosition);

        endTime = time;
        DetectSwipe();
    }

    /// <summary>
    /// Detects if input could be considered a swipe
    /// </summary>
    void DetectSwipe()
    {
        if(Vector3.Distance(startPosition, endPosition) >= minimumDistance && (endTime - startTime) <= maximumTime)
        {
            // We're in here if the input is considered a swipe!
            Debug.DrawLine(startPosition, endPosition, Color.red, 5f);

            Vector2 direction = (endPosition - startPosition).normalized;
            SwipeDirection(direction);
        }
    }

    /// <summary>
    /// Determine the direction of the already determined swipe
    /// </summary>
    /// <param name="direction">Of determined swipe</param>
    void SwipeDirection(Vector2 direction)
    {
        if(Vector2.Dot(Vector2.up, direction) > directionThreshold)
        {
            print("Swiped UP "); // Do something on up etc!
        }
        else if (Vector2.Dot(Vector2.down, direction) > directionThreshold)
        {
            print("Swiped DOWN");
        }
        else if (Vector2.Dot(Vector2.left, direction) > directionThreshold)
        {
            print("Swiped LEFT");
        }
        else if (Vector2.Dot(Vector2.right, direction) > directionThreshold)
        {
            print("Swiped RIGHT");
        }
    }
}
