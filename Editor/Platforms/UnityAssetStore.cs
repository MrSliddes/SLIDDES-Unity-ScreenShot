using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Screenshot.Platforms
{
    public class UnityAssetStore : Platform
    {
        public override void Invoke()
        {
            // Set display name
            displayName = "Unity Asset Store";
            // Set resolutions
            resolutions = new List<Resolution>()
            {
                new Resolution(-1, -1, "Camera Resolution", false),
                new Resolution(160, 160, "Icon Image 160x160", false),
                new Resolution(420, 280, "Card Image 420x280", false),
                new Resolution(1950, 1300, "Cover Image 1950x1300", false),
                new Resolution(1200, 630, "Social Media Image 1200x630", false),
                new Resolution(1200, 1200, "Screenshot 1200x1200", false),
                new Resolution(2400, 1600, "Screenshot 2400x1600", false)
            };
        }
    }
}