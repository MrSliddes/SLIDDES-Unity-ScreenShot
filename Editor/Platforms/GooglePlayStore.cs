using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Screenshot.Platforms
{
    public class GooglePlayStore : Platform
    {
        public override void Invoke()
        {
            // Set display name
            displayName = "Google Play Store";
            // Set resolutions
            resolutions = new List<Resolution>()
            {
                new Resolution(-1, -1, "Camera Resolution", false),
                new Resolution(512, 512, "App Icon 512x512", false),
                new Resolution(1024, 500, "Function Header 1024x500", false),
                new Resolution(1080, 1920, "Phone Portrait 1080x1920"),
                new Resolution(1920, 1080, "Phone Landscape 1920x1080", false),
                new Resolution(2000, 1300, "Phone Portrait 2000x1300", false),
                new Resolution(3840, 2160, "7-inch Tablet 16:9 3840x2160", false),
                new Resolution(2160, 3840, "7-inch Tablet 9:16 2160x3840"),
                new Resolution(3840, 2160, "10-inch Tablet 16:9 3840x2160", false),
                new Resolution(2160, 3840, "10-inch Tablet 9:16 2160x3840")
            };
        }
    }
}
