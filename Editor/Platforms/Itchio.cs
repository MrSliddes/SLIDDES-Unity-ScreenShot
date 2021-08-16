using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Screenshot.Platforms
{
    public class Itchio : Platform
    {
        public override void Invoke()
        {
            // Set display name
            displayName = "itch.io";
            // Set resolutions
            resolutions = new List<Resolution>()
            {
                new Resolution(-1, -1, "Camera Resolution", false),
                new Resolution(630, 500, "Cover Image Recommended 630x500"),
                new Resolution(315, 250, "Cover Image Minimal 315x250", false),
                new Resolution(3840, 2160, "Cover Image Maximal 3840x2160", false),
                new Resolution(960, 400, "Page Banner 960x400", false),
                new Resolution(1920, 1080, "Landscape Screenshot 16:9 1920x1080"),
                new Resolution(1200, 900, "Landscape Screenshot 4:3 1200x900"),
                new Resolution(900, 1200, "Portrait Screenshot 3:4 900x1200"),
                new Resolution(800, 1131, "Portrait Screenshot 1:1.414 800:1131"),
                new Resolution(1131, 800, "Landscape Screenshot 1.414:1 1131x800"),
                new Resolution(1034, 800, "Landscape Screenshot 10:7.7 1034x800"),
                new Resolution(800, 1034, "Portrait Screenshot 7.7:10 800:1034"),
            };
        }
    }
}