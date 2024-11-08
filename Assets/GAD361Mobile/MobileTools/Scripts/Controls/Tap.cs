using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Murdoch.GAD361.MobileTools
{

    public class Tap : MobileControls
    {
        // Update is called once per frame
        void Update()
        {
            if (!controlsEnabled)
                return;
            bool tapped = false;
            Vector2 tappos = Vector2.zero;
            if (MobileGameManager.IsDesktop)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    tapped = true;
                    tappos = Input.mousePosition;
                }
            }
            else
            {
                //1. Do this first touch
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0); //first touch only!
                    if (touch.phase == TouchPhase.Began)
                    {
                        tapped = true;
                        tappos = touch.position;
                    }
                }
            }

            if (tapped)
            {
                if (worldSpaceTouchPositions)
                {
                    tappos = Camera.main.ScreenToWorldPoint(tappos);
                }
                if (OnTap != null)
                {
                    OnTap(tappos);
                }
            }
        }
    }
}