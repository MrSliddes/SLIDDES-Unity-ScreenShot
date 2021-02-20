using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace SLIDDES.Editor.Window
{
    public class EditorWindowScreenShot : EditorWindow
    {
        // TODO
        // snip selected for screenshotsize
        // Saving for file extension
        // button that takes screenshots for android and iOS formats
        // include UI button toggle

        /// <summary>
        /// When an screenshot is taken the editor should highlight it if possible
        /// </summary>
        private bool highlightTakenScreenShot;

        /// <summary>
        /// Use custom screenshot settings
        /// </summary>
        private bool useCustomSettings;
        /// <summary>
        /// The rect of screenshot (startpoint x, startpoint y, width, height)
        /// </summary>
        private RectInt screenShotRect = new RectInt(0, 0, 1920, 1080);
        /// <summary>
        /// Custom file directory
        /// </summary>
        private string customFileDirectory;
        /// <summary>
        /// Custom file name
        /// </summary>
        private string customFileName;
        /// <summary>
        /// Custom file extension the file is saved as (PNG, JPG)
        /// </summary>
        private ScreenShotFileExtension customScreenShotFileExtension = ScreenShotFileExtension.png;
        /// <summary>
        /// Custom texture format used to save Texture2D
        /// </summary>
        private TextureFormat customTextureFormat = TextureFormat.RGBA32;
        /// <summary>
        /// Use a custom camera selected by user
        /// </summary>
        private bool useCustomCamera;
        /// <summary>
        /// If usingCustomCamera the camera is stored in here
        /// </summary>
        private Camera customCamera;

        // GUILayout Editor window values
        private readonly int editorSpacePixels = 10;
        private readonly int editorMarginLeft = 15;
        private readonly int editorMarginRight = 30; // Double of left!
        private readonly Vector2 editorWindowMinSize = new Vector2(500, 140);
        /// <summary>
        /// Used for editor scrollbar
        /// </summary>
        private Vector2 editorScrollPosition;
        /// <summary>
        /// Used to store previous style
        /// </summary>
        private FontStyle previousStyle;

        private bool editorFoldoutScreenShot = true;
        private bool editorFoldoutSettings;
        private bool editorFoldoutCustomSettings;

        //
        /// <summary>
        /// Is user able to take screenshot og gameview?
        /// </summary>
        private bool canTakeScreenShotGameView = true;
        /// <summary>
        /// Is user able to take screenshot of sceneview?
        /// </summary>
        private bool canTakeScreenShotSceneView = true;

        private string fileDirectoryDefault = "/SLIDDES Temporary/Editor/";

        // Add menu item to the Window menu
        [MenuItem("Window/SLIDDES/ScreenShot")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow window = EditorWindow.GetWindow(typeof(EditorWindowScreenShot), false, "SLIDDES ScreenShot"); // Name
            window.minSize = new Vector2(500, 140);
        }

        private void Awake()
        {
            // Load stored settings
            highlightTakenScreenShot = EditorPrefs.GetBool("highlightTakenScreenShot", true);
            useCustomSettings = EditorPrefs.GetBool("useCustomSettings", false);
            screenShotRect = new RectInt(EditorPrefs.GetInt("screenShotRectX", 0), EditorPrefs.GetInt("screenShotRectY", 0), EditorPrefs.GetInt("screenShotRectW", 1920), EditorPrefs.GetInt("screenShotRectH", 1080));
            customFileDirectory = EditorPrefs.GetString("customFileDirectory", Application.dataPath + "/SLIDDES Temporary");
            customFileName = EditorPrefs.GetString("customFileName", "{0}_{1}_{2}x{3}");
            //customScreenShotFileExtension
            //customTextureFormat
            useCustomCamera = EditorPrefs.GetBool("useCustomCamera", false);
            //customCamera
        }

        private void OnDestroy()
        {
            // Save settings
            EditorPrefs.SetBool("highlightTakenScreenShot", highlightTakenScreenShot);
            EditorPrefs.SetBool("useCustomSettings", useCustomSettings);
            EditorPrefs.SetInt("screenShotRectX", screenShotRect.x); EditorPrefs.SetInt("screenShotRectY", screenShotRect.y); EditorPrefs.SetInt("screenShotRectW", screenShotRect.width); EditorPrefs.SetInt("screenShotRectH", screenShotRect.height);
            EditorPrefs.SetString("customFileDirectory", customFileDirectory);
            EditorPrefs.SetString("customFileName", customFileName);
            //customScreenShotFileExtension
            //customTextureFormat
            EditorPrefs.SetBool("useCustomCamera", useCustomCamera);
            //customCamera
        }

        private void OnGUI()
        {
            // Window code goes here
            EditorGUIUtility.labelWidth = 200; // Distance between text and buttons, etc.
            EditorGUILayout.BeginVertical(); // Make it look like unity inspector
            editorScrollPosition = EditorGUILayout.BeginScrollView(editorScrollPosition);
            GUILayout.Space(editorSpacePixels);

            // Screenshot
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            previousStyle = EditorStyles.foldout.fontStyle; EditorStyles.foldout.fontStyle = FontStyle.Bold; // Bold foldout
            editorFoldoutScreenShot = EditorGUILayout.Foldout(editorFoldoutScreenShot, " Take ScreenShot", true);
            EditorStyles.foldout.fontStyle = previousStyle; // Reset bold foldout
            if(editorFoldoutScreenShot)
            {
                EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);

                // Check if possible to take screenshot gameview
                if(!canTakeScreenShotGameView) GUI.enabled = false;

                // Button to take a screenshot of game view
                if(GUILayout.Button("Screenshot Game View", GUILayout.Height(30)))
                {
                    Camera targetCamera;
                    if(useCustomSettings && useCustomCamera)
                    {
                        if(customCamera == null)
                        {
                            Debug.LogError("[SLIDDES Screeshot] Error, no custom camera detected. Please assign a custom camera");
                            return;
                        }
                        targetCamera = customCamera;
                    }
                    else
                    {
                        if(Camera.main == null)
                        {
                            Debug.LogError("[SLIDDES Screeshot] Error, no camera detected. Please (create a camera and) assign the tag MameraCain to the camera you want to take a screenshot with ");
                            return;
                        }
                        targetCamera = Camera.main;
                    }

                    // Take a screenshot of the camera view
                    RenderTexture renderTexture;
                    Vector2Int resolution;
                    if(useCustomSettings)
                    {
                        resolution = new Vector2Int(screenShotRect.width, screenShotRect.height);
                    }
                    else
                    {
                        resolution = new Vector2Int(targetCamera.pixelWidth, targetCamera.pixelHeight);
                    }
                    renderTexture = new RenderTexture(resolution.x, resolution.y, 24);
                    RenderTexture prevTexture = targetCamera.targetTexture; // If camera has texture copy it
                    targetCamera.targetTexture = renderTexture;
                    // Texture 2D
                    Texture2D screenShot;
                    if(useCustomSettings)
                    {
                        screenShot = new Texture2D(resolution.x - screenShotRect.x, resolution.y - screenShotRect.y, customTextureFormat, false);
                    }
                    else
                    {
                        screenShot = new Texture2D(resolution.x, resolution.y, TextureFormat.RGBA32, false);
                    }

                    targetCamera.Render(); // change cam
                    RenderTexture.active = renderTexture;

                    // Custom Rect
                    if(useCustomSettings)
                    {
                        screenShot.ReadPixels(new Rect(screenShotRect.x, screenShotRect.y, resolution.x, resolution.y), 0, 0);
                    }
                    else
                    {
                        screenShot.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);
                    }


                    // if cam had target text reset it back
                    targetCamera.targetTexture = prevTexture;
                    RenderTexture.active = null;
                    DestroyImmediate(renderTexture); // Not in runtime

                    byte[] bytes;
                    // File extension
                    if(useCustomSettings)
                    {
                        switch(customScreenShotFileExtension)
                        {
                            case ScreenShotFileExtension.png: bytes = screenShot.EncodeToPNG(); break;
                            case ScreenShotFileExtension.jpg: bytes = screenShot.EncodeToJPG(); break;
                            case ScreenShotFileExtension.EXR: bytes = screenShot.EncodeToEXR(Texture2D.EXRFlags.CompressZIP); break;
                            case ScreenShotFileExtension.TGA: bytes = screenShot.EncodeToTGA(); break;
                            default: bytes = screenShot.EncodeToPNG(); break;
                        }
                    }
                    else
                    {
                        // Default PNG
                        bytes = screenShot.EncodeToPNG();
                    }
                    string fileName = GenerateScreenShotName(resolution.x, resolution.y, true);
                    string filePath; if(useCustomSettings) filePath = fileName; else filePath = Application.dataPath + fileName;
                    // Check for directory in asset root folder
                    if(!Directory.Exists(Application.dataPath + fileDirectoryDefault)) // change destination
                    {
                        //if it doesn't, create it
                        Directory.CreateDirectory(Application.dataPath + fileDirectoryDefault);
                    }
                    System.IO.File.WriteAllBytes(filePath, bytes);
                    AssetDatabase.Refresh(); // Refresh so picture becomes visable in project files inside unity

                    // Highlight screenshot in Project folder
                    if(!useCustomSettings && highlightTakenScreenShot)
                    {
                        var a = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets" + fileName, typeof(Texture2D)); // Its a texture2D so look for a texture2D
                        EditorGUIUtility.PingObject(a);
                    }

                    Debug.Log("[SLIDDES Screenshot] Took screenshot of game view to: " + filePath);
                }

                // Enable GUI if buttons where disabled
                GUI.enabled = true;

                // Check if possible to take screenshot gameview
                if(!canTakeScreenShotSceneView) GUI.enabled = false;

                // Button to take screenshot of scene view
                if(GUILayout.Button("Screenshot Scene View", GUILayout.Height(30)))
                {
                    var sceneViewCamera = UnityEditor.EditorWindow.GetWindow<SceneView>().camera;

                    // Take a screenshot of the camera view
                    RenderTexture renderTexture;
                    // Set resolution
                    Vector2Int resolution;
                    if(useCustomSettings)
                    {
                        resolution = new Vector2Int(screenShotRect.width, screenShotRect.height);
                    }
                    else
                    {
                        resolution = new Vector2Int(Camera.main.pixelWidth, Camera.main.pixelHeight);
                    }
                    renderTexture = new RenderTexture(resolution.x, resolution.y, 24); // Feature: changable depth
                    // Copy rendertext if it contained one..
                    RenderTexture prevTexture = sceneViewCamera.targetTexture;
                    sceneViewCamera.targetTexture = renderTexture;
                    // Texture 2D
                    Texture2D screenShot;
                    if(useCustomSettings)
                    {
                        screenShot = new Texture2D(resolution.x - screenShotRect.x, resolution.y - screenShotRect.y, customTextureFormat, false);
                    }
                    else
                    {
                        screenShot = new Texture2D(resolution.x, resolution.y, TextureFormat.RGBA32, false);
                    }

                    sceneViewCamera.Render(); // change cam
                    RenderTexture.active = renderTexture;

                    // Custom Rect
                    if(useCustomSettings)
                    {
                        screenShot.ReadPixels(new Rect(screenShotRect.x, screenShotRect.y, resolution.x, resolution.y), 0, 0);
                    }
                    else
                    {
                        screenShot.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);
                    }

                    sceneViewCamera.targetTexture = prevTexture; // if cam had target text reset it back
                    RenderTexture.active = null;
                    DestroyImmediate(renderTexture); // Not in runtime

                    byte[] bytes;
                    // File extension
                    if(useCustomSettings)
                    {
                        switch(customScreenShotFileExtension)
                        {
                            case ScreenShotFileExtension.png: bytes = screenShot.EncodeToPNG(); break;
                            case ScreenShotFileExtension.jpg: bytes = screenShot.EncodeToJPG(); break;
                            case ScreenShotFileExtension.EXR: bytes = screenShot.EncodeToEXR(); break;
                            case ScreenShotFileExtension.TGA: bytes = screenShot.EncodeToTGA(); break;
                            default: bytes = screenShot.EncodeToPNG(); break;
                        }
                    }
                    else
                    {
                        // Default PNG
                        bytes = screenShot.EncodeToPNG();
                    }
                    string fileName = GenerateScreenShotName(resolution.x, resolution.y, false);
                    string filePath; if(useCustomSettings) filePath = fileName; else filePath = Application.dataPath + fileName;
                    // Check for directory in asset root folder
                    if(!Directory.Exists(Application.dataPath + fileDirectoryDefault)) // change destination
                    {
                        //if it doesn't, create it
                        Directory.CreateDirectory(Application.dataPath + fileDirectoryDefault);
                    }
                    System.IO.File.WriteAllBytes(filePath, bytes);
                    AssetDatabase.Refresh(); // Refresh so picture becomes visable in project files inside unity

                    // Highlight screenshot in Project folder
                    if(!useCustomSettings && highlightTakenScreenShot)
                    {
                        var a = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets" + fileName, typeof(Texture2D)); // Its a texture2D so look for a texture2D
                        EditorGUIUtility.PingObject(a);
                    }

                    Debug.Log("[SLIDDES Screenshot] Took screenshot of Scene view to: " + filePath);
                }

                // Enable GUI if buttons where disabled
                GUI.enabled = true;

                // Helpbox file directory
                if(useCustomSettings)
                {
                    // Custom file directory
                    // Check if directory exists
                    if(System.IO.Directory.Exists(customFileDirectory))
                    {
                        EditorGUILayout.HelpBox("Screenshots are saved at: " + customFileDirectory, MessageType.Info);
                        canTakeScreenShotGameView = true;
                        canTakeScreenShotSceneView = true;
                    }
                    else
                    {
                        // Display warning message => displays at File Directory
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Screenshots are saved at: " + Application.dataPath + fileDirectoryDefault, MessageType.None);
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            // Settings
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            previousStyle = EditorStyles.foldout.fontStyle; EditorStyles.foldout.fontStyle = FontStyle.Bold; // Bold foldout
            editorFoldoutSettings = EditorGUILayout.Foldout(editorFoldoutSettings, " Settings", true);
            EditorStyles.foldout.fontStyle = previousStyle; // Reset bold foldout
            if(editorFoldoutSettings)
            {
                EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
                // Highlight Taken Screenshot
                if(useCustomSettings)
                {
                    GUI.enabled = false;
                    highlightTakenScreenShot = EditorGUILayout.Toggle("Highlight Taken ScreenShot", highlightTakenScreenShot);
                    GUI.enabled = true;
                }
                else
                {
                    highlightTakenScreenShot = EditorGUILayout.Toggle("Highlight Taken ScreenShot", highlightTakenScreenShot);
                }
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("For UI to render on Game view the UI Render Mode needs to be on Screen Space - Camera", MessageType.Info);
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            // Custom Settings            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            previousStyle = EditorStyles.foldout.fontStyle; EditorStyles.foldout.fontStyle = FontStyle.Bold; // Bold foldout
            editorFoldoutCustomSettings = EditorGUILayout.Foldout(editorFoldoutCustomSettings, " Custom Settings", true);
            EditorStyles.foldout.fontStyle = previousStyle; // Reset bold foldout
            if(editorFoldoutCustomSettings)
            {
                EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
                useCustomSettings = EditorGUILayout.Toggle("Use Custom Settings", useCustomSettings);

                if(useCustomSettings)
                {
                    // ScreenShot size
                    EditorGUILayout.BeginHorizontal();
                    var b = EditorGUIUtility.wideMode;
                    EditorGUIUtility.wideMode = Screen.width > editorWindowMinSize.x - editorMarginLeft - editorMarginRight;
                    screenShotRect = EditorGUILayout.RectIntField("ScreenShot Size", screenShotRect);
                    EditorGUIUtility.wideMode = b;
                    if(GUILayout.Button("Reset", GUILayout.Width(45)))
                    {
                        screenShotRect = new RectInt(0, 0, 1920, 1080);
                        Repaint();
                    }
                    EditorGUILayout.EndHorizontal();

                    // File directory
                    EditorGUILayout.BeginHorizontal();
                    customFileDirectory = EditorGUILayout.TextField("File Directory", customFileDirectory);
                    if(string.IsNullOrEmpty(customFileDirectory) || string.IsNullOrWhiteSpace(customFileDirectory)) customFileDirectory = Application.dataPath + fileDirectoryDefault;
                    if(GUILayout.Button("Reset", GUILayout.Width(45)))
                    {
                        customFileDirectory = Application.dataPath + fileDirectoryDefault;
                        canTakeScreenShotGameView = canTakeScreenShotSceneView = true;
                        Repaint();
                    }
                    EditorGUILayout.EndHorizontal();
                    // Check if directory exists
                    if(System.IO.Directory.Exists(customFileDirectory))
                    {
                        canTakeScreenShotGameView = true;
                        canTakeScreenShotSceneView = true;
                    }
                    else
                    {
                        // Display warning message
                        EditorGUILayout.HelpBox("File Directory not found at: " + customFileDirectory, MessageType.Error);
                        canTakeScreenShotGameView = false;
                        canTakeScreenShotSceneView = false;
                    }

                    // File name
                    EditorGUILayout.BeginHorizontal();
                    customFileName = EditorGUILayout.TextField("File Name", customFileName);
                    if(string.IsNullOrEmpty(customFileName) || string.IsNullOrWhiteSpace(customFileName)) customFileName = "{0}_{1}_{2}x{3}";
                    if(GUILayout.Button("Reset", GUILayout.Width(45)))
                    {
                        customFileName = "{0}_{1}_{2}x{3}";
                        Repaint();
                    }
                    EditorGUILayout.EndHorizontal();
                    if(customFileName == "{0}_{1}_{2}x{3}") EditorGUILayout.HelpBox("View Type, DateTime.Now, Width, Height", MessageType.None);

                    // File extension
                    customScreenShotFileExtension = (ScreenShotFileExtension)EditorGUILayout.EnumPopup("File Extension", customScreenShotFileExtension);
                    if(customScreenShotFileExtension == ScreenShotFileExtension.EXR)
                    {
                        // customTextureFormat has to be RGBAFloat
                        customTextureFormat = TextureFormat.RGBAFloat;
                    }

                    // TextureFormat
                    EditorGUILayout.BeginHorizontal();
                    if(customScreenShotFileExtension == ScreenShotFileExtension.EXR) GUI.enabled = false;
                    customTextureFormat = (TextureFormat)EditorGUILayout.EnumPopup("Texture Format", customTextureFormat);
                    if(GUILayout.Button("Reset", GUILayout.Width(45)))
                    {
                        customTextureFormat = TextureFormat.RGBA32;
                        Repaint();
                    }
                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();
                    if(customScreenShotFileExtension == ScreenShotFileExtension.EXR)
                    {
                        EditorGUILayout.HelpBox("File Extension EXR only works with Texture Format RGBAFloat.", MessageType.Info);
                    }

                    // Custom camera
                    EditorGUILayout.LabelField("Custom Camera");
                    useCustomCamera = EditorGUILayout.Toggle("Use Custom Camera", useCustomCamera);
                    if(useCustomCamera)
                    {
                        customCamera = (Camera)EditorGUILayout.ObjectField("", customCamera, typeof(Camera), true);
                        if(customCamera == null)
                        {
                            canTakeScreenShotGameView = false;
                            EditorGUILayout.HelpBox("No camera selected. Please select a camera", MessageType.Error);
                        }
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Generates the screenshot name with filedirectory (fileDirectory + name)
        /// </summary>
        /// <param name="width">Width in pixels of screenshot</param>
        /// <param name="height">Height in pixels of screenshot</param>
        /// <param name="isGameView">Is the view that of window Game?</param>
        /// <returns></returns>
        private string GenerateScreenShotName(int width, int height, bool isGameView) // editable name
        {
            string s;
            if(isGameView) s = "GameView"; else s = "SceneView";
            // Custom file name
            string name;
            if(useCustomSettings) name = customFileName; else name = "{0}_{1}_{2}x{3}";
            // Result
            string result;
            if(useCustomSettings)
            {
                result = string.Format(customFileDirectory + name, s, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"), width, height);
            }
            else
            {
                result = string.Format(fileDirectoryDefault + name, s, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"), width, height);
            }
            if(useCustomSettings)
            {
                switch(customScreenShotFileExtension)
                {
                    case ScreenShotFileExtension.png: result += ".png"; break;
                    case ScreenShotFileExtension.jpg: result += ".jpg"; break;
                    case ScreenShotFileExtension.EXR: result += ".exr"; break;
                    case ScreenShotFileExtension.TGA: result += ".tga"; break;
                    default: result += ".png"; break;
                }
            }
            else
            {
                result += ".png";
            }
            return result;
        }
    }

    public enum ScreenShotFileExtension
    {
        [InspectorName("PNG")]
        png,
        [InspectorName("JPG")]
        jpg,
        [InspectorName("EXR")]
        EXR,
        [InspectorName("TGA")]
        TGA
    }
}

// Links
// https://answers.unity.com/questions/22954/how-to-save-a-picture-take-screenshot-from-a-camer.html
// Copy component values https://answers.unity.com/questions/530178/how-to-get-a-component-from-an-object-and-add-it-t.html?_ga=2.100223772.388525063.1612813523-1054575301.1586788407
// Label width https://answers.unity.com/questions/630488/editorguilayoutobjectfield-redundant-space-between.html
// Wide mode (vector3 layout) https://docs.unity3d.com/ScriptReference/EditorGUIUtility-wideMode.html
// Editor style https://forum.unity.com/threads/how-to-recreate-this-editor-style.496392/