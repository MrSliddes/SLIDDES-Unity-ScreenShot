using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Screenshot.Platforms
{
    public class Steam : Platform
    {
        public override void Invoke()
        {
            // Set display name
            displayName = "Steam";
            // Set resolutions
            resolutions = new List<Resolution>()
            {
                new Resolution(-1, -1, "Camera Resolution", false),
                new Resolution(460, 215, "Header Capsule 460x215"),
                new Resolution(231, 87, "Small Capsule 231x87"),
                new Resolution(616, 353, "Main Capsule 616x353"),
                new Resolution(374, 448, "Hero Capsule 374x448"),
                new Resolution(1920, 1080, "Screenshot 16:9"),
                new Resolution(707, 232, "Bundle Header 707x232", false),

                new Resolution(32, 32, "Community Icon 32x32", false),
                new Resolution(32, 32, "Client Icon 32x32", false),
                new Resolution(184, 69, "Community Small Capsule 184x69", false),

                new Resolution(600, 900, "Library Capsule 600x900", false),
                new Resolution(3840, 1240, "Library Hero 3840x1240", false),
                new Resolution(1280, 720, "Library Logo 1280x720", false),

                new Resolution(800, 450, "Event Cover 800x450", false),
                new Resolution(1920, 622, "Event Header 1920x622", false),
                new Resolution(2108, 460, "Library Spotlight 2108x460", false),
            };
        }
    }
}