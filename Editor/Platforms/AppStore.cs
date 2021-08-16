using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Screenshot.Platforms
{
    public class AppStore : Platform
    {
        public override void Invoke()
        {
            // Set display name
            displayName = "App Store";
            // Set resolutions
            resolutions = new List<Resolution>()
            {
                new Resolution(-1, -1, "Camera Resolution", false),
                new Resolution(512, 512, "App Icon 512x512", false),
                new Resolution(1242, 2688, "iPhone 6.5\" Portrait 1242x2688"),
                new Resolution(2688, 1242, "iPhone 6.5\" Landscape 2688x1242", false),
                new Resolution(1125, 2436, "iPhone 5.8\" Portrait 1125x2436"),
                new Resolution(2436, 1125, "iPhone 5.8\" Landscape 2436x1125", false),
                new Resolution(1242, 2208, "iPhone 5.5\" Portrait 1242x2208"),
                new Resolution(2208, 1242, "iPhone 5.5\" Landscape 2208x1242", false),
                new Resolution(750, 1334, "iPhone 4.7\" Portrait 750x1334"),
                new Resolution(1334, 750, "iPhone 4.7\" Landscape 1334x750", false),
                new Resolution(640, 1096, "iPhone 4\" Portrait - status bar 640x1096"),
                new Resolution(640, 1136, "iPhone 4\" Portrait + status bar 640x1136", false),
                new Resolution(1136, 600, "iPhone 4\" Landscape - status bar 1136x600", false),
                new Resolution(1136, 640, "iPhone 4\" Landscape + status bar 1136x640", false),
                new Resolution(640, 920, "iPhone 3.5\" Portrait - status bar 640x920"),
                new Resolution(640, 960, "iPhone 3.5\" Portrait + status bar 640x960", false),
                new Resolution(960, 600, "iPhone 3.5\" Landscape - status bar 960x600", false),
                new Resolution(960, 640, "iPhone 3.5\" Landscape + status bar 960x640", false),
                new Resolution(2048, 2732, "iPad 12.9\" Portrait 2048x2732"),
                new Resolution(2732, 2048, "iPad 12.9\" Landscape 2732x2048", false),
                new Resolution(1668, 2388, "iPad 11\" Portrait 1668x2388"),
                new Resolution(2388, 1668, "iPad 11\" Landscape 2388x1668", false),
                new Resolution(1668, 2224, "iPad 10.5\" Portrait 1668x2224"),
                new Resolution(2224, 1668, "iPad 10.5\" Landscape 2224x1668", false),
                new Resolution(1536, 2008, "iPad 9.7\" HR Portrait - status bar 1536x2008"),
                new Resolution(1536, 2048, "iPad 9.7\" HR Portrait + status bar 1536x2048", false),
                new Resolution(2048, 1496, "iPad 9.7\" HR Landscape - status bar 2048x1496", false),
                new Resolution(2048, 1536, "iPad 9.7\" HR Landscape + status bar 2048x1536", false),
                new Resolution(768, 1004, "iPad 9.7\" SR Portrait - status bar 768x1004", false),
                new Resolution(768, 1024, "iPad 9.7\" SR Portrait + status bar 768x1024", false),
                new Resolution(1024, 748, "iPad 9.7\" SR Landscape - status bar 1024x748", false),
                new Resolution(1024, 768, "iPad 9.7\" SR Landscape + status bar 1024x768", false),
                new Resolution(1280, 800, "Mac 16:10 1280x800", false),
                new Resolution(1440, 900, "Mac 16:10 1440x900", false),
                new Resolution(2560, 1600, "Mac 16:10 2560x1600", false),
                new Resolution(2880, 1800, "Mac 16:10 2880x1800", false),
                new Resolution(1920, 1800, "Apple TV 1920x1080", false),
                new Resolution(3840, 2160, "Apple TV 3840x2160", false),
                new Resolution(312, 390, "Apple Watch (S3) 312x390", false),
                new Resolution(368, 448, "Apple Watch (S4) 368x448", false)
            };
        }
    }
}

