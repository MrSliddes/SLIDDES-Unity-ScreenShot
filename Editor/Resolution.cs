using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLIDDES.Screenshot
{
    /// <summary>
    /// Resolution class for a screenshot
    /// </summary>
    public class Resolution
    {
        public int X { get { return size.x; } }
        public int Y { get { return size.y; } }

        /// <summary>
        /// Does this resolution get included when generating screenshots?
        /// </summary>
        public bool include = true;
        /// <summary>
        /// The display name shown in the inspector
        /// </summary>
        public string displayName;
        /// <summary>
        /// The size of the resolution
        /// </summary>
        public Vector2Int size;

        public Resolution(int x, int y, bool include = true)
        {
            this.size = new Vector2Int(x, y);
            displayName = string.Format("{0}x{1}", x, y);
            this.include = include;
        }

        public Resolution(int x, int y, string displayName, bool include = true)
        {
            this.size = new Vector2Int(x, y);
            this.displayName = displayName;
            this.include = include;
        }
    }
}