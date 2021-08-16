using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Screenshot.Platforms
{
    public class GitHub : Platform
    {
        public override void Invoke()
        {
            // Set display name
            displayName = "GitHub";
            // Set resolutions
            resolutions = new List<Resolution>()
            {
                new Resolution(-1, -1, "Camera Resolution", false),
                new Resolution(838, 100, "838x100", false),
                new Resolution(838, 200, "838x200", false),
                new Resolution(838, 300, "838x300", false),
                new Resolution(838, 400, "838x400", false),
                new Resolution(838, 500, "838x500", false),
                new Resolution(838, 600, "838x600", false),
                new Resolution(838, 700, "838x700", false),
                new Resolution(838, 800, "838x800", false),
                new Resolution(838, 210, "README.md Img 1:0.25 838x210"),
                new Resolution(838, 419, "README.md Img 1:0.5 838x419"),
                new Resolution(838, 629, "README.md Img 1:0.75 838x629"),
                new Resolution(838, 838, "README.md Img 1:1 838x838"),
            };
        }
    }
}