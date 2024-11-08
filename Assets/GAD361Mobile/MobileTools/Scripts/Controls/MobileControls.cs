using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Murdoch.GAD361.MobileTools
{
    /**
    * SwipeData is a small object which is passed to a swipe delegate when
    * a swipe occurs. Currently it contains a Direction.Dir to indicate
    * which direction was swiped.
    * It will be extended in future to include other data.
    */
    public class SwipeData
    {
        public Direction.Dir dir;
        public Vector2 posStart; //where this swipe started
        public Vector2 posCurrent; //current position of swipe
        //could also have extra data here for 
        //- length/velocity of swipe
        //- position on screen where swipe occurred
    }


    /**
     * MobileControls is a base class for any touch controls for mobile.
     * It includes delegates for 
     * @see OnTap for tap input
     * @see OnTilt for tilt input
     * @see OnSwipe for swipes
     * and @see OnTouch for multitouch
     */
    public class MobileControls : MonoBehaviour
    {
        //public delegate void OnTap(Action<Vector2> pos);
        //static public event Action<Vector2> OnTap = delegate{};
        public delegate void PosDelegate(Vector2 pos);
        public delegate void SwipeDelegate(SwipeData swipe);
        public delegate void TouchArrayDelegate(Vector3[] touches);
        public delegate void TiltDelegate(Vector3 tilt);
        public static PosDelegate OnTap;
        public static TiltDelegate OnTilt;
        public static SwipeDelegate OnSwipe;
        public static TouchArrayDelegate OnTouch;

        /**
        * Whether or not controls are enabled: each delegate should respect
        * the value of controlsEnabled and ignore input if false.
        */
        public static bool controlsEnabled = true; //disables touch controls
        
        /**
        * This flag indicates whether reported positions should be in
        * screen space (the default) or world space (set this to true)
        */
        [Tooltip("Should reported positions be in world space or screen space?")]
        public bool worldSpaceTouchPositions = false; //in screenspace or world space?


    }
}
