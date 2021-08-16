using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Screenshot.Platforms
{
    public class Twitter : Platform
    {
        public override void Invoke()
        {
            // Set display name
            displayName = "Twitter";
            // Set resolutions
            resolutions = new List<Resolution>()
            {
                new Resolution(-1, -1, "Camera Resolution", false),
                new Resolution(400, 400, "Profile Picture 400x400", false),
                new Resolution(1500, 500, "Profile Header 1500x500", false),
                new Resolution(1200, 675, "Image 1200x675", false),
                new Resolution(1200, 628, "Card 1200x628", false),
                new Resolution(120, 120, "Summary Card 120x120", false),
                new Resolution(640, 360, "Media Player Card 640x360", false),
                new Resolution(160, 160, "Product Card 160x160", false),
                new Resolution(800, 200, "Lead Generation Card 800x200", false),
                new Resolution(800, 320, "Website Card 800x320", false)
            };
        }
    }
}