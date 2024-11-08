﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Murdoch.GAD361.MobileTools
{
    public class TiltControl : MobileControls
    {
        Vector3 lastTilt;
        public bool alwaysPublish; //always publish accel values, even if same.
        // Start is called before the first frame update
        void Start()
        {
            lastTilt = Vector3.zero;
        }

        // Update is called once per frame
        void Update()
        {
            if (!controlsEnabled)
                return;

            //For desktop, V and H axis control X and Z respectively, Y is 0
            if (MobileGameManager.IsDesktop)
            {
                Vector3 keysTilt = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
                if (alwaysPublish || keysTilt != lastTilt)
                {
                    if (OnTilt != null)
                    {
                        OnTilt(keysTilt);
                    }
                    lastTilt = keysTilt;
                }
            }
            else
            {
                if (alwaysPublish || Input.acceleration != lastTilt)
                {
                    if (OnTilt != null)
                    {
                        OnTilt(Input.acceleration);
                    }
                    lastTilt = Input.acceleration;
                }
            }
        }
    }
}