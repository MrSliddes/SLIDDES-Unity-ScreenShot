using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Screenshot.Platforms
{
    public class EpicGamesStore : Platform
    {
        public override void Invoke()
        {
            // Set display name
            displayName = "Epic Games Store";
            // Set resolutions
            resolutions = new List<Resolution>()
            {
                new Resolution(-1, -1, "Camera Resolution", false),
                new Resolution(1200, 1600, "Store Carousel Thumbnail 1200x1600", false),

                new Resolution(360, 480, "Store Game Cover 360x480"),
                new Resolution(640, 854, "Store Game Cover 640x854"),
                new Resolution(854, 480, "Store Game Cover 854x480"),
                new Resolution(1280, 720, "Store Game Cover 1280x720"),
                new Resolution(1920, 1080, "Game Screenshot 1920x1080")
            };
        }
    }
}