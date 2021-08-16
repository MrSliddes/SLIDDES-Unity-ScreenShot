using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Screenshot.Platforms
{
    public class GOG : Platform
    {
        public override void Invoke()
        {
            // Set display name
            displayName = "GOG";
            // Set resolutions
            resolutions = new List<Resolution>()
            {
                new Resolution(-1, -1, "Camera Resolution", false),
                new Resolution(342, 482, "Box Cover 342x482", false),
                new Resolution(1024, 1024, "Icon 1024x1024", false),
                new Resolution(98, 98, "Web Icon 98x98", false),
                new Resolution(876, 360, "Installer 876x360", false),

                new Resolution(1600, 740, "Main Game Art 1600x740", false),
                new Resolution(794, 395, "Main Logo 794x395", false),
                new Resolution(2560, 670, "Main Galaxy Background 2560x670", false),

                new Resolution(1550, 490, "News 1550x490", false),

                new Resolution(1461, 676, "Social Twitter 1461x676", false),
                new Resolution(1046, 492, "Social Facebook 1046x492", false),
            };
        }
    }
}
