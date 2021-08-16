using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Screenshot.Platforms
{
    public class ArtStation : Platform
    {
        public override void Invoke()
        {
            // Set display name
            displayName = "ArtStation";
            // Set resolutions
            resolutions = new List<Resolution>()
            {
                new Resolution(-1, -1, "Camera Resolution", false),
                new Resolution(2400, 2400, "1:1 2400x2400"),
                new Resolution(3000, 3000, "1:1 3000x3000"),
                new Resolution(3600, 3600, "1:1 3600x3600"),
                new Resolution(4800, 4800, "1:1 4800x4800"),
                new Resolution(6000, 6000, "1:1 6000x6000"),
                new Resolution(7200, 7200, "1:1 7200x7200"),
                new Resolution(9000, 9000, "1:1 9000x9000"),
                new Resolution(10800, 10800, "1:1 10800x10800"),
                new Resolution(2400, 3000, "4:5 Portrait 2400x3000"),
                new Resolution(4800, 6000, "4:5 Portrait 4800x6000"),
                new Resolution(7200, 9000, "4:5 Portrait 7200x9000"),
                new Resolution(3000, 2400, "5:4 Landscape 3000x2400"),
                new Resolution(6000, 4800, "5:4 Landscape 6000x4800"),
                new Resolution(9000, 7200, "5:4 Landscape 9000x7200"),
                new Resolution(3600, 4800, "3:4 Portrait 3600x4800"),
                new Resolution(5400, 7200, "3:4 Portrait 5400x7200"),
                new Resolution(9000, 12000, "3:4 Portrait 9000x12000"),
                new Resolution(4800, 3600, "4:3 Landscape 4800x3600"),
                new Resolution(7200, 5400, "4:3 Landscape 7200x5400"),
                new Resolution(12000, 9000, "4:3 Landscape 12000x9000"),
                new Resolution(3600, 5400, "2:3 Portrait 3600x5400"),
                new Resolution(4800, 7200, "2:3 Portrait 4800x7200"),
                new Resolution(7200, 10800, "2:3 Portrait 7200x10800"),
                new Resolution(9600, 14400, "2:3 Portrait 9600x14400"),
                new Resolution(5400, 3600, "3:2 Landscape 5400x3600"),
                new Resolution(7200, 4800, "3:2 Landscape 7200x4800"),
                new Resolution(9000, 6000, "3:2 Landscape 9000x6000"),
                new Resolution(10800, 7200, "3:2 Landscape 10800x7200"),
                new Resolution(14400, 9600, "3:2 Landscape 14400x9600"),
                new Resolution(3000, 6000, "1:2 Portrait 3000x6000"),
                new Resolution(6000, 3000, "2:1 Landscape 6000x3000")
            };
        }
    }
}