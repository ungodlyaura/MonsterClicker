using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Murdoch.GAD361.MobileTools
{

    public class SwipeDetectorPlus : MobileControls
    {

        [Tooltip("Minimum distance in pixels needed for swipe")]
        public float minSwipeDistance = 25.0f; //pixels
        [Tooltip("If true, unsuccessful swipes are counted as taps")]
        public bool nonSwipeIsTap = false;

        Vector2 starttouch;
        Vector2 endtouch;
        
        // Update is called once per frame
        void Update()
        {
            if (!controlsEnabled)
                return;
            if (MobileGameManager.IsDesktop)
            {
                float v = 0; // Input.GetAxisRaw("Vertical");
                float h = 0; // Input.GetAxisRaw("Horizontal");
                if (Input.GetKeyUp(KeyCode.UpArrow))
                {
                    v = 1;
                }
                else if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    v = -1;
                }
                else if (Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    h = -1;
                }
                else if (Input.GetKeyUp(KeyCode.RightArrow))
                {
                    h = 1;
                }
                Vector2 spos = Vector2.zero;
                Vector2 epos = new Vector2(h, v) * minSwipeDistance;
                //Debug.Log(epos);
                CheckSwipe(spos, epos);
                if (nonSwipeIsTap && Input.GetMouseButton(0))
                {
                    if (OnTap != null)
                    {
                        Vector2 p = Input.mousePosition;
                        if (worldSpaceTouchPositions)
                            p = Camera.main.ScreenToWorldPoint(p);
                        
                        if (OnTap != null)
                        {
                            OnTap(p);
                        }
                    }
                }
            }
            else
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        starttouch = touch.position;
                        endtouch = touch.position;
                    }

                    if (touch.phase == TouchPhase.Moved) //(if !detectAfterRelease)
                    {
                        endtouch = touch.position;
                        CheckSwipe(starttouch, endtouch);
                    }

                    if (touch.phase == TouchPhase.Ended)
                    {
                        endtouch = touch.position;
                        bool swiped = CheckSwipe(starttouch, endtouch);
                        if (!swiped && nonSwipeIsTap && OnTap != null)
                        {
                            if (worldSpaceTouchPositions)
                                endtouch = Camera.main.ScreenToWorldPoint(endtouch);
                            if (OnTap != null)
                            {
                                OnTap(endtouch);
                            }
                        }
                    }
                }
            }
            
        }

        bool CheckSwipe(Vector2 s_pos, Vector2 e_pos)
        {
            float vdist = Mathf.Abs(s_pos.y - e_pos.y);
            float hdist = Mathf.Abs(s_pos.x - e_pos.x);
            bool isVerticalSwipe = (vdist > hdist);
            bool swipeDistMet = ((vdist >= minSwipeDistance) || (hdist >= minSwipeDistance));
            if (swipeDistMet)
            {
                SwipeData sdata = new SwipeData();
                if (isVerticalSwipe)
                {
                    if (e_pos.y > s_pos.y)
                    {
                        sdata.dir = Direction.Dir.UP;
                    }
                    else
                    {
                        sdata.dir = Direction.Dir.DOWN;
                    }
                }
                else
                {
                    if (e_pos.x > s_pos.x)
                    {
                        sdata.dir = Direction.Dir.RIGHT;
                    }
                    else
                    {
                        sdata.dir = Direction.Dir.LEFT;
                    }
                }
                if (OnSwipe != null)
                {
                    sdata.posStart = s_pos;
                    sdata.posCurrent = e_pos;
                    if (worldSpaceTouchPositions)
                    {
                        sdata.posStart = Camera.main.ScreenToWorldPoint(s_pos);
                        sdata.posCurrent = Camera.main.ScreenToWorldPoint(e_pos);
                    }
                    if (OnSwipe != null)
                    {
                        OnSwipe(sdata);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}