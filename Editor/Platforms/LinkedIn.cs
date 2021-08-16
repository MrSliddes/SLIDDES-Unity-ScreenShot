using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Screenshot.Platforms
{
    public class LinkedIn : Platform
    {
        public override void Invoke()
        {
            // Set display name
            displayName = "LinkedIn";
            // Set resolutions
            resolutions = new List<Resolution>()
            {
                new Resolution(-1, -1, "Camera Resolution", false),
                new Resolution(300, 300, "Logo Image 300x300"),
                new Resolution(1128, 191, "Cover Image 1128x191"),
                new Resolution(1128, 376, "Main Image 1128x376"),
                new Resolution(502, 282, "Custom Modules 502x282"),
                new Resolution(264, 176, "Company Photos 264x176", false),
                new Resolution(900, 600, "Company Photos 900x600")
            };
        }
    }
}