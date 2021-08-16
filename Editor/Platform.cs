using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace SLIDDES.Screenshot
{
    /// <summary>
    /// Base class for a platform (pc/mac/linux/etc)
    /// </summary>
    public abstract class Platform
    {
        /// <summary>
        /// The current selected resolution for this platform
        /// </summary>
        public Resolution CurrentResolution { get { return resolutions[screenshotResolutionIndex]; } }

        /// <summary>
        /// The selected resolution index when taking a single screenshot
        /// </summary>
        public int screenshotResolutionIndex;
        /// <summary>
        /// The display name in the inspector of the platform
        /// </summary>
        public string displayName;
        /// <summary>
        /// The resolutions for screenshots associated with this platform
        /// </summary>
        public List<Resolution> resolutions;
        /// <summary>
        /// The reordarable list for resolutions
        /// </summary>
        public ReorderableList reorderableListResolutions;

        public Platform() 
        {
            Init();
        }

        public void Init()
        {
            Invoke();
            Setup();
        }

        public abstract void Invoke();

        /// <summary>
        /// Setup the reorderable list
        /// </summary>
        public void Setup()
        {
            // Reorderable list https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/GUI/ReorderableList.cs
            // Initialize reorderable list
            reorderableListResolutions = new ReorderableList(resolutions, typeof(Resolution), true, false, true, true);

            // Override reorderable list callback
            reorderableListResolutions.drawHeaderCallback = (Rect rect) =>
            {
                //EditorGUI.LabelField(rect, "Screenshot Resolutions");
            };

            reorderableListResolutions.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                // Bool include
                resolutions[index].include = EditorGUI.ToggleLeft(new Rect(rect.x, rect.y, 50, EditorGUIUtility.singleLineHeight), new GUIContent("", "Include this resolution?"), resolutions[index].include);
                // X and Y value
                float w = EditorGUIUtility.labelWidth; EditorGUIUtility.labelWidth = 40;
                resolutions[index].size.x = EditorGUI.IntField(new Rect(rect.x + 30, rect.y, 80, EditorGUIUtility.singleLineHeight), new GUIContent("Width", "The width of the screenshot"), resolutions[index].size.x);
                resolutions[index].size.y = EditorGUI.IntField(new Rect(rect.x + 130, rect.y, 80, EditorGUIUtility.singleLineHeight), new GUIContent("Height", "The height of the screenshot"), resolutions[index].size.y);
                EditorGUIUtility.labelWidth = w;
                // Name
                EditorGUI.LabelField(new Rect(rect.x + 230, rect.y, rect.width - 230, EditorGUIUtility.singleLineHeight), resolutions[index].displayName);
            };

            reorderableListResolutions.onAddCallback = (list) =>
            {
                reorderableListResolutions.list.Add(new Resolution(1, 1));
            };
        }
    }
}