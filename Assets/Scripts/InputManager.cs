using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] Blade blade;

#if !UNITY_STANDALONE
    Vector2 startingTouch;
    bool isSwiping = false;
#endif

    void Update()
    {
        // Use mouse input in editor or computer
//#if UNITY_EDITOR || UNITY_STANDALONE
        // if (Input.GetMouseButtonDown(0))
        // {
        //     blade.StartCutting();
        // }
        // else if (Input.GetMouseButtonUp(0))
        // {
        //     blade.StopCutting();
        // }
//#else
//        // Use touch input on mobile
//        if (Input.touchCount == 1)
//        {
//			if(isSwiping)
//			{
//				Vector2 diff = Input.GetTouch(0).position - startingTouch;

//				// Put difference in Screen ratio, but using only width, so the ratio is the same on both
//                // axes (otherwise we would have to swipe more vertically...)
//				diff = new Vector2(diff.x/Screen.width, diff.y/Screen.width);

//				if(diff.magnitude > 0.01f) //we set the swipe distance to trigger cutting to 1% of the screen width
//				{
//					isSwiping = false;
//				}
//            }

//        	// Input check is AFTER the swip test, that way if TouchPhase.Ended happen a single frame after the Began Phase
//			// a swipe can still be registered (otherwise, isSwiping will be set to false and the test wouldn't happen for that began-Ended pair)
//			if(Input.GetTouch(0).phase == TouchPhase.Began)
//			{
//                blade.StartCutting();
//				startingTouch = Input.GetTouch(0).position;
//				isSwiping = true;
//			}
//			else if(Input.GetTouch(0).phase == TouchPhase.Ended)
//			{
//                blade.StopCutting();
//				isSwiping = false;
//			}
//        }
//#endif
    }
}
