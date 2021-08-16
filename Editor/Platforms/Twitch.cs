using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Screenshot.Platforms
{
    public class Twitch : Platform
    {
        public override void Invoke()
        {
            // Set display name
            displayName = "Twitch";
            // Set resolutions
            resolutions = new List<Resolution>()
            {
                new Resolution(-1, -1, "Camera Resolution", false),
                new Resolution(600, 800, "Box Art 600x800")
            };
        }
    }
}