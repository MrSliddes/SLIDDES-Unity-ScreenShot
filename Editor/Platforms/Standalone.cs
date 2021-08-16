using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Screenshot.Platforms
{
    public class Standalone : Platform
    {
        public override void Invoke()
        {
            // Set display name
            displayName = "Standalone";
            // Set resolutions
            resolutions = new List<Resolution>()
            {
                new Resolution(-1, -1, "Camera Resolution"),
                new Resolution(640, 480),
                new Resolution(720, 480),
                new Resolution(720, 576),
                new Resolution(800, 600),
                new Resolution(1024, 768),
                new Resolution(1152, 864),
                new Resolution(1176, 664),
                new Resolution(1280, 720),
                new Resolution(1280, 768),
                new Resolution(1280, 800),
                new Resolution(1280, 960),
                new Resolution(1280, 1024),
                new Resolution(1360, 768),
                new Resolution(1366, 768),
                new Resolution(1400, 1050),
                new Resolution(1440, 900),
                new Resolution(1600, 900),
                new Resolution(1600, 1024),
                new Resolution(1680, 1050),
                new Resolution(1920, 1080)
            };
        }
    }
}