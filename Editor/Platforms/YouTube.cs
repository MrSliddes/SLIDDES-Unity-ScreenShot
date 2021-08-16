using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Screenshot.Platforms
{
    public class YouTube : Platform
    {
        public override void Invoke()
        {
            // Set display name
            displayName = "YouTube";
            // Set resolutions
            resolutions = new List<Resolution>()
            {
                new Resolution(-1, -1, "Camera Resolution", false),
                new Resolution(800, 800, "Channel Icon 800x800", false),
                new Resolution(2560, 1440, "Channel Art 2560x1440", false),
                new Resolution(2560, 423, "Desktop Display 2560x423"),
                new Resolution(1855, 423, "Tablet Display 1855x423"),
                new Resolution(1546, 423, "Mobile Display 1546x423"),
                new Resolution(1280, 720, "Video Thumbnail 1280x720", false),
                new Resolution(300, 250, "Display Ad 300x250", false),
                new Resolution(300, 60, "Display Ad Long 300x60", false),
                new Resolution(480, 70, "Overlay Ad 480x70", false)
            };
        }
    }
}