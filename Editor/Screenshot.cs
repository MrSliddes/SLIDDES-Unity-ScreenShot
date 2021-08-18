using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Unity.EditorCoroutines.Editor;

namespace SLIDDES.Screenshot
{
    /// <summary>
    /// The editor window class for screenshot
    /// </summary>
    public class Screenshot : EditorWindow
    {
        public static Screenshot Instance
        {
            get
            {
                if(instance == null)
                {
                    GetWindow(typeof(Screenshot), false, "Screenshot");
                }
                return instance;
            }
            private set { }
        }

        /// <summary>
        /// The current selected platform to take screenshots for
        /// </summary>
        public Platform CurrentPlatform { get { return platforms[currentPlatformIndex]; } }

        private static Screenshot instance;

        /// <summary>
        /// Is a screenshot currently being processed?
        /// </summary>
        private bool isProcessingScreenshot;
        /// <summary>
        /// Next time the editor window is opend reset all values (it skips load())
        /// </summary>
        private bool resetOnOpen;
        /// <summary>
        /// Show the loading bar when generating multiple screenshots
        /// </summary>
        private bool showLoadingBar = true;
        /// <summary>
        /// Show a message when a screenshot has been taken
        /// </summary>
        private bool showScreenshotMessage = true;
        /// <summary>
        /// The index of the current platform in platforms []
        /// </summary>
        private int currentPlatformIndex;
        /// <summary>
        /// The editor window to screenshot array index
        /// </summary>
        private int currentEditorWindowIndex;
        /// <summary>
        /// The file directory where the screenshots are saved
        /// </summary>
        private string fileDirectory = "";
        /// <summary>
        /// The default file directory path based on operating system (Images/SLIDDES/Screenshots/)
        /// </summary>
        private string fileDirectoryDefault;
        /// <summary>
        /// The display name of each platform
        /// </summary>
        private string[] platformsDisplayNames;
        /// <summary>
        /// The current selected file extension for a screenshot
        /// </summary>
        private FileExtension fileExtension = FileExtension.jpg;
        /// <summary>
        /// The texture format of the screenshot
        /// </summary>
        private TextureFormat textureFormat = TextureFormat.RGBA32;
        /// <summary>
        /// The camera the screenshot gets taken with
        /// </summary>
        private Camera camera;
        /// <summary>
        /// Array of all the platforms
        /// </summary>
        private Platform[] platforms;
        /// <summary>
        /// Queue of screenshots to process
        /// </summary>
        private Queue<Options> toTake = new Queue<Options>();
        /// <summary>
        /// The render texture used for screenshots
        /// </summary>
        private RenderTexture renderTexture;
        /// <summary>
        /// Render texture used to store pre screenshot target texture of camera
        /// </summary>
        private RenderTexture preTargetTexture;
        /// <summary>
        /// Render texture used to store pre screenshot RenderTexture.Active
        /// </summary>
        private RenderTexture preRenderTextureActive;
        /// <summary>
        /// The screenshot texture2D
        /// </summary>
        private Texture2D texture2D;

        private readonly string EDITORPREF_PREFIX = "SLIDDES_Unity_Screenshot_";

        // Editor
        private bool foldoutQuickAccess = true;
        private bool foldoutPlatforms;
        private bool foldoutResolutions;
        private bool foldoutEditorWindows;
        private bool foldoutSettings;
        private Vector2 editorScrollPosition;

        [MenuItem("Window/SLIDDES/Screenshot")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow window = GetWindow(typeof(Screenshot), false, "Screenshot");
            window.minSize = new Vector2(300, 145);
            window.titleContent.tooltip = "SLIDDES Screenshot, create screenshots with ease";
        }

        private void OnEnable()
        {
            // Get
            if(instance != null)
            {
                UnityEngine.Debug.Log("[Screenshot] is already active in scene.");
                Close();
                return;
            }
            instance = this;

            // Set
            // Platforms
            platforms = new Platform[]
            {
                new Platforms.Standalone(),
                new Platforms.ArtStation(),
                new Platforms.AppStore(),
                new Platforms.EpicGamesStore(),
                new Platforms.GitHub(),
                new Platforms.GOG(),
                new Platforms.GooglePlayStore(),
                new Platforms.Itchio(),
                new Platforms.LinkedIn(),
                new Platforms.Steam(),
                new Platforms.Twitch(),
                new Platforms.Twitter(),
                new Platforms.UnityAssetStore(),
                new Platforms.YouTube()
            };
            platforms.Initialize();
            platformsDisplayNames = platforms.Select(x => x.displayName).ToArray();

            // File directory default based on operating system
#if UNITY_EDITOR_WIN
            fileDirectoryDefault = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\SLIDDES\\Screenshots\\Unity";
#elif UNITY_EDITOR_OSX
            fileDirectoryDefault = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "/SLIDDES/Screenshots/Unity";
#endif
            if(fileDirectory.Length == 0) fileDirectory = fileDirectoryDefault;

            // Load values unless reset is active, then skip load
            resetOnOpen = EditorPrefs.GetBool(EDITORPREF_PREFIX + "resetOnOpen", false);
            if(!resetOnOpen) Load(); else resetOnOpen = false;

            camera = Camera.main;

            EditorCoroutineUtility.StartCoroutineOwnerless(CheckForUnprocessedScreenshots());
        }

        private void OnDisable()
        {
            Save();
        }

        private void OnDestroy()
        {
            Save();
        }

        private void OnGUI()
        {
            editorScrollPosition = EditorGUILayout.BeginScrollView(editorScrollPosition);
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            OnGUIQuickAccess();
            OnGUIPlatforms();
            OnGUIEditorWindows();
            OnGUISettings();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.EndScrollView();
        }

        #region OnGUI Functions

        private void OnGUIQuickAccess()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foldoutQuickAccess = EditorGUILayout.Foldout(foldoutQuickAccess, new GUIContent("Quick Access", "Quickly take a screenshot of Game/Scene view."), true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });
            if(foldoutQuickAccess)
            {
                if(GUILayout.Button(new GUIContent("Screenshot Game View", "Take a screenshot of the Game view window."), GUILayout.Height(32)))
                {
                    TakeScreenshot(new Options(1, true, false));
                }

                if(GUILayout.Button(new GUIContent("Screenshot Scene View", "Take a screenshot of the Scene view window."), GUILayout.Height(32)))
                {
                    TakeScreenshot(new Options(0, true, false));
                }

                if(GUILayout.Button(new GUIContent("Open Screenshot Folder", "Open the folder location where screenshots are saved."), GUILayout.Height(24)))
                {
#if UNITY_EDITOR_WIN
                    Process.Start("explorer.exe", @fileDirectory);
#elif UNITY_EDITOR_OSX
                    //TODO
#endif
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void OnGUIPlatforms()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foldoutPlatforms = EditorGUILayout.Foldout(foldoutPlatforms, new GUIContent("Platforms", "For taking platform specific screenshots."), true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });
            if(foldoutPlatforms)
            {
                // Select current platform
                currentPlatformIndex = EditorGUILayout.Popup(new GUIContent("Current Platform", "Select the platform to take screenshots for."), currentPlatformIndex, platformsDisplayNames);
                // Display current platform options
                EditorGUILayout.Space();
                                
                GUILayout.Label(new GUIContent("Single Screenshot", "The buttons for taking a single screenshot"), new GUIStyle(EditorStyles.boldLabel));
                // Screenshot game view with resolution
                if(GUILayout.Button(new GUIContent("Screenshot Game View", "Take a screenshot of the Game view window."), new GUILayoutOption[] { GUILayout.Height(28) }))
                {
                    TakeScreenshot(new Options(1));
                }
                // Screenshot scene view with resolution
                if(GUILayout.Button(new GUIContent("Screenshot Scene View", "Take a screenshot of the Scene view window."), GUILayout.Height(28)))
                {
                    TakeScreenshot(new Options(0));
                }
                // Screenshot game view with resolution and UI
                if(GUILayout.Button(new GUIContent("Screenshot Game View + UI", "Take a screenshot of the Game view window with UI. Will always be a .png with texture format RGB32A."), GUILayout.Height(28)))
                {
                    TakeScreenshot(new Options(1, true));
                }
                // Screenshot scene view with resolution? and gizmos
                if(GUILayout.Button(new GUIContent("Screenshot Scene View + Gizmos", "Take a screenshot of the Scene view window with Gizmos. Make sure the Scene view is visable and that no other editor window is covering it."), GUILayout.Height(28)))
                {
                    TakeScreenshot(new Options(0, true));
                }
                // Select current platform resolution
                CurrentPlatform.screenshotResolutionIndex = EditorGUILayout.Popup(new GUIContent("Selected Resolution", "Select the resolution to take the screenshot with.\n\nYou can add a custom resolution at \"Avaiable Resolutions\". \"Camera Resolution\" means it takes the current camera resolution.\n\nResolution does not apply to scene view."), CurrentPlatform.screenshotResolutionIndex, CurrentPlatform.resolutions.Select(x => x.displayName).ToArray());
                EditorGUILayout.Space();

                GUILayout.Label(new GUIContent("Multiple Screenshots", "The buttons for taking a multiple screenshots"), new GUIStyle(EditorStyles.boldLabel));
                // Generate screenshots game view with selected resolutions
                if(GUILayout.Button(new GUIContent("Generate Screenshots Game View", "Take a screenshot for every selected resolution of the Game view window."), GUILayout.Height(28)))
                {
                    // For each included platform resolution create a screenshot
                    Resolution[] res = CurrentPlatform.resolutions.Where(x => x.include).ToArray();
                    foreach(var item in res)
                    {
                        UnityEngine.Debug.Log(item.size);
                    }
                    TakeScreenshots(1, res);
                }
                // Generate screenshots game view + UI with selected resolutions
                if(GUILayout.Button(new GUIContent("Generate Screenshots Game View + UI", "Take a screenshot for every selected resolution of the Game view window with UI."), GUILayout.Height(28)))
                {
                    // For each included platform resolution create a screenshot
                    TakeScreenshots(1, CurrentPlatform.resolutions.Where(x => x.include).ToArray(), true);
                }
                // Select resolutions to include when generating screenshots
                foldoutResolutions = EditorGUILayout.Foldout(foldoutResolutions, new GUIContent("Available Resolutions", "All available resolutions for this platform that you can include or not when generating multiple screenshots."), true);
                if(foldoutResolutions)
                {
                    CurrentPlatform.reorderableListResolutions.DoLayoutList();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void OnGUIEditorWindows()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foldoutEditorWindows = EditorGUILayout.Foldout(foldoutEditorWindows, new GUIContent("Editor Windows", "Take a screenshot of an editor window."), true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });
            if(foldoutEditorWindows)
            {
                EditorWindow[] ew = Resources.FindObjectsOfTypeAll<EditorWindow>();
                currentEditorWindowIndex = EditorGUILayout.Popup(new GUIContent("Editor Window", "The editor window to screenshot"), currentEditorWindowIndex, ew.Select(x => x.titleContent.text).ToArray());
                EditorGUILayout.Space();

                // Screenshot selected editor window
                if(GUILayout.Button(new GUIContent("Screenshot Editor Window", "Take a screenshot of the selected editor window.\n\nMake sure the editor window is visable and that nothing overlaps it."), GUILayout.Height(32)))
                {
                    EditorWindow activeWindow = ew[currentEditorWindowIndex];
                    if(!activeWindow.hasFocus)
                    {
                        activeWindow.Show();
                        UnityEngine.Debug.LogWarning("[Screenshot] Make sure " + activeWindow.titleContent.text + " is visible first");
                        return;
                    }

                    // Get screen position and sizes
                    Vector2 vec2Position = activeWindow.position.position;
                    var sizeX = activeWindow.position.width;
                    var sizeY = activeWindow.position.height + 20;

                    // Read pixels at given position sizes
                    Color[] colors = InternalEditorUtility.ReadScreenPixel(vec2Position, (int)sizeX, (int)sizeY);

                    // Write result Color[] data into a temporal Texture2D
                    var result = new Texture2D((int)sizeX, (int)sizeY);
                    result.SetPixels(colors);

                    // Encode texture2D
                    byte[] bytes = Instance.EncodeTexture2D(result);

                    // In order to avoid bloading Texture2D into memory destroy it
                    UnityEngine.Object.DestroyImmediate(result);

                    // Save
                    File.WriteAllBytes(Path.Combine(Instance.fileDirectory, Instance.GetFileName((int)sizeX, (int)sizeY, true, false, "EW_" + activeWindow.titleContent.text)), bytes);

                    if(Instance.showScreenshotMessage) UnityEngine.Debug.Log("[Screenshot] Took screenshot");
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void OnGUISettings()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foldoutSettings = EditorGUILayout.Foldout(foldoutSettings, new GUIContent("Settings", "Change stuff to fit your needs."), true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });
            if(foldoutSettings)
            {
                // Camera
                camera = (Camera)EditorGUILayout.ObjectField(new GUIContent("Camera", "The camera the screenshot gets taken with. Only applies to screenshots taken within the Game View."), camera, typeof(Camera), true);
                if(camera == null)
                {
                    camera = Camera.main;
                    if(camera == null) EditorGUILayout.HelpBox("No camera selected / detected. Please select a camera.", MessageType.Error);
                }
                // File directory
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("File Directory", "The file directory where the screenshots are saved."));
                if(GUILayout.Button(new GUIContent("", EditorGUIUtility.IconContent("Folder Icon").image, "Select folder to save screenshots in."), new GUILayoutOption[] { GUILayout.Height(20), GUILayout.Width(32)}))
                {
                    fileDirectory = EditorUtility.OpenFolderPanel("Select Screenshot Folder", fileDirectory, "");
                    if(fileDirectory.Length == 0) fileDirectory = fileDirectoryDefault;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.HelpBox("Screenshots are saved at:\n" + fileDirectory, MessageType.None);
                // File extension
                fileExtension = (FileExtension)EditorGUILayout.Popup(new GUIContent("File Extension", "The extension the file is saved with"), (int)fileExtension, Enum.GetNames(typeof(FileExtension)).Select(x => "." + x).ToArray());
                // Texture format
                textureFormat = (TextureFormat)EditorGUILayout.EnumPopup(new GUIContent("Texture Format", "The texture format of the screenshot. Default is RGBA32."), textureFormat);
                // Show loading bar //TODO not working when generating multiple screenshots
                //showLoadingBar = EditorGUILayout.Toggle(new GUIContent("Show Loading Bar", "Show the loading bar when generating multiple screenshots."), showLoadingBar);
                // Show screenshot message
                showScreenshotMessage = EditorGUILayout.Toggle(new GUIContent("Show Messages", "Show a message when a screenshot has been taken."), showScreenshotMessage);

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                // Reset values
                if(GUILayout.Button(new GUIContent("Reset Screenshot Values", "Reset all saved values to default.")))
                {
                    if(EditorUtility.DisplayDialog("Reset Screenshot Values", "Are you sure you want to reset all screenshot values? This cannot be undone.", "Reset", "Cancel"))
                    {
                        resetOnOpen = true;
                        Close();
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

#endregion

        /// <summary>
        /// For taking a screenshot
        /// </summary>
        /// <param name="options">The options for the screenshot</param>
        public static void TakeScreenshot(Options options)
        {
            Instance.toTake.Enqueue(options);
        }

        /// <summary>
        /// For taking multiple screenshots
        /// </summary>
        /// <param name="cameraIndex">What camera to take the screenshot from. 0 = scene view, 1 = game view, 2 = custom</param>
        /// <param name="resolutions">The resolutions of the screenshots</param>
        /// <param name="includeUI">To include the camera UI / Gizmos</param>
        public static void TakeScreenshots(int cameraIndex, Resolution[] resolutions, bool includeUI = false)
        {
            for(int i = 0; i < resolutions.Length; i++)
            {
                TakeScreenshot(new Options(cameraIndex, resolutions[i].size, includeUI));
            }
            UnityEngine.Debug.Log("[Screenshot] Generated " + resolutions.Length + " screenshots");
        }

        /// <summary>
        /// Processes the screenshot with given options. !Use TakeScreenshot instead of this if you dont know what this does!
        /// </summary>        
        /// <param name="options">The options for the screenshot</param>
        public static IEnumerator ProcessScreenshot(Options options)
        {
            Instance.isProcessingScreenshot = true;
            // Variables needed
            byte[] bytes;

            // Get the targetCamera
            Camera targetCamera;
            switch(options.cameraIndex)
            {
                case 0: targetCamera = GetWindow<SceneView>().camera; break;
                case 1:
                    // Check if settings camera is the same as main camera, else it is a custom selected camera = change options.cameraIndex to 2
                    // We have to check for this since there isnt a button to take a screenshot with custom camera
                    targetCamera = Instance.camera;
                    if(targetCamera != Camera.main)
                    {
                        // Custom camera
                        options.cameraIndex = 2;
                    }
                    break;
                default: targetCamera = Instance.camera; break;
            }
            if(targetCamera == null)
            {
                UnityEngine.Debug.LogWarning("[Screenshot] Couldn't take screenshot: Screenshot camera not assigned. (Check Screenshot > Settings > Camera)");
                Instance.isProcessingScreenshot = false;
                yield break;
            }

            // Save pre screenshot rect values
            Rect preRect = targetCamera.rect;
            Rect prePixelRect = targetCamera.pixelRect;

            // Get resolution for camera and screenshot
            Vector2Int resolution = Instance.GetCameraAndScreenshotResolution(options, targetCamera);

            // For including UI
            if(options.includeUI)
            {
                // For game view use Screencapture. cons: always png, cant customize
                // For scene view read screen pixels. cons: scene view has to be visable, editor windows can overlap

                switch(options.cameraIndex)
                {
                    case 0: // Scene camera with Gizmos
                        EditorWindow activeWindow = GetWindow<SceneView>();
                        if(!activeWindow.hasFocus)
                        {
                            activeWindow.Show();
                            UnityEngine.Debug.LogWarning("[Screenshot] Make sure Scene view is visible first");
                            Instance.isProcessingScreenshot = false;
                            yield break;
                        }

                        // Resize camera -> cannot resize scene view camera, cannot change window size as that causes black screens
                        //Instance.SetCameraResolution(targetCamera, resolution);

                        // Get screen position and sizes, cutout top banner
                        Vector2 vec2Position = activeWindow.position.position;
                        vec2Position.x += 1;
                        vec2Position.y += 40;
                        var sizeX = activeWindow.position.width;
                        var sizeY = activeWindow.position.height - 21;

                        // Read pixels at given position sizes
                        Color[] colors = InternalEditorUtility.ReadScreenPixel(vec2Position, (int)sizeX, (int)sizeY);                        

                        // Write result Color[] data into a temporal Texture2D
                        var result = new Texture2D((int)sizeX, (int)sizeY);
                        result.SetPixels(colors);

                        // Encode texture2D
                        bytes = Instance.EncodeTexture2D(result);

                        // In order to avoid bloading Texture2D into memory destroy it
                        UnityEngine.Object.DestroyImmediate(result);

                        // Save
                        File.WriteAllBytes(Path.Combine(Instance.fileDirectory, Instance.GetFileName((int)sizeX, (int)sizeY)), bytes);
                        break;
                    case 1: // Game camera with UI
                        // Set game view resolution with help of Kyusyukeigo.Helper.GameViewSizeHelper
                        Kyusyukeigo.Helper.GameViewSizeHelper.AddCustomSize(Kyusyukeigo.Helper.GameViewSizeHelper.GetCurrentGameViewSizeGroupType(), Kyusyukeigo.Helper.GameViewSizeHelper.GameViewSizeType.FixedResolution, resolution.x, resolution.y, "SDSsnstTemp");
                        Kyusyukeigo.Helper.GameViewSizeHelper.ChangeGameViewSize(Kyusyukeigo.Helper.GameViewSizeHelper.GetCurrentGameViewSizeGroupType(), Kyusyukeigo.Helper.GameViewSizeHelper.GameViewSizeType.FixedResolution, resolution.x, resolution.y, "SDSsnstTemp");

                        // Set pixelRect first, then rect
                        targetCamera.pixelRect = new Rect(0, 0, resolution.x, resolution.y);
                        yield return null;
                        targetCamera.rect = new Rect(0, 0, 1, 1);
                        // Make sure the UI updated with the new res
                        Canvas.ForceUpdateCanvases();
                        EditorApplication.ExecuteMenuItem("Window/General/Game");

                        // Wait a frame
                        yield return null;
                        //UnityEngine.Debug.Log("pre s " + targetCamera.rect + " " + targetCamera.pixelRect);

                        // Capture and save screenshot
                        UnityEngine.ScreenCapture.CaptureScreenshot(Path.Combine(Instance.fileDirectory, Instance.GetFileName(resolution.x, resolution.y, false) + ".png"));
                        // Make sure game view is visible so the screenshot gets processed
                        EditorApplication.ExecuteMenuItem("Window/General/Game");

                        // Wait a frame
                        yield return null;

                        Kyusyukeigo.Helper.GameViewSizeHelper.RemoveCustomSize(Kyusyukeigo.Helper.GameViewSizeHelper.GetCurrentGameViewSizeGroupType(), Kyusyukeigo.Helper.GameViewSizeHelper.GameViewSizeType.FixedResolution, resolution.x, resolution.y, "SDSsnstTemp");

                        // Restore post screenshot values, important that pixelRect gets reset first
                        targetCamera.pixelRect = prePixelRect;
                        yield return null;
                        targetCamera.rect = preRect;
                        Canvas.ForceUpdateCanvases();
                        yield return null;
                        break;
                    default: // Custom camera with UI
                        // Need to switch the main camera tag to targetCamera (regardless if its already main or not)
                        Camera preMainCamera = Camera.main;
                        string preTargetCameraTag = targetCamera.tag;
                        preMainCamera.tag = "Untagged";
                        targetCamera.tag = "MainCamera";

                        // Set targetCamera resolution
                        Instance.SetCameraResolution(targetCamera, resolution);

                        // Capture and save screenshot
                        UnityEngine.ScreenCapture.CaptureScreenshot(Path.Combine(Instance.fileDirectory, Instance.GetFileName(options.resolution.x, options.resolution.y, false) + ".png"));

                        // Restore post screenshot values
                        targetCamera.rect = preRect;
                        targetCamera.pixelRect = prePixelRect;
                        preMainCamera.tag = "MainCamera";
                        targetCamera.tag = preTargetCameraTag;

                        // Make sure game view is visible so the screenshot gets processed
                        EditorApplication.ExecuteMenuItem("Window/General/Game");
                        break;
                }

                if(Instance.showScreenshotMessage) UnityEngine.Debug.Log("[Screenshot] Took screenshot");
                Instance.isProcessingScreenshot = false;
                yield break;
            }

            // Save pre screenshot texture values
            Instance.preTargetTexture = targetCamera.targetTexture;
            Instance.preRenderTextureActive = RenderTexture.active;                    

            // Set targetCamera resolution
            Instance.SetCameraResolution(targetCamera, resolution);

            // Texture for the screenshot
            Instance.renderTexture = new RenderTexture(resolution.x, resolution.y, 24);
            targetCamera.targetTexture = Instance.renderTexture;
            RenderTexture.active = targetCamera.targetTexture;
            targetCamera.Render();

            // Texture2D
            Instance.texture2D = new Texture2D(resolution.x, resolution.y, Instance.textureFormat, false);

            // Read the active render texture to texture2D
            Instance.texture2D.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);

            // Restore post screenshot values
            targetCamera.targetTexture = Instance.preTargetTexture;
            RenderTexture.active = Instance.preRenderTextureActive;
            targetCamera.rect = preRect;
            targetCamera.pixelRect = prePixelRect;

            // Convert texture2D into bytes            
            bytes = Instance.EncodeTexture2D(Instance.texture2D);

            // File name
            string fileName = Instance.GetFileName(resolution.x, resolution.y);

            // File directory (check if it exists)
            if(!Directory.Exists(Instance.fileDirectory))
            {
                UnityEngine.Debug.Log("[Screenshot] Created a new file directory at: " + Instance.fileDirectory);
                Directory.CreateDirectory(Instance.fileDirectory);
            }

            // Save
            File.WriteAllBytes(Path.Combine(Instance.fileDirectory, fileName), bytes);

            if(Instance.showScreenshotMessage) UnityEngine.Debug.Log("[Screenshot] Took screenshot");
            Instance.isProcessingScreenshot = false;
            yield break;
        }
        
        /// <summary>
        /// Checks the queue for screenshots to process
        /// </summary>
        /// <returns>null</returns>
        private IEnumerator CheckForUnprocessedScreenshots()
        {
            while(true)
            {
                if(toTake.Count > 0)
                {                    
                    if(!isProcessingScreenshot)
                    {
                        // Process screenshot and dequeue
                        EditorCoroutineUtility.StartCoroutineOwnerless(ProcessScreenshot(toTake.Dequeue()));
                        Instance.isProcessingScreenshot = true;
                    }
                }

                yield return null;
            }
        }

        /// <summary>
        /// Encode a texture2D to byte array
        /// </summary>
        /// <param name="texture2D">The texture to convert</param>
        /// <returns>byte[]</returns>
        private byte[] EncodeTexture2D(Texture2D texture2D)
        {
            switch(Instance.fileExtension)
            {
                case FileExtension.jpg: return texture2D.EncodeToJPG();
                case FileExtension.png: return texture2D.EncodeToPNG();
                case FileExtension.exr: return texture2D.EncodeToEXR();
                case FileExtension.tga: return texture2D.EncodeToTGA();
                default: return texture2D.EncodeToJPG();
            }
        }

        /// <summary>
        /// Get the file name for a screenshot
        /// </summary>
        /// <param name="width">Width of the screenshot</param>
        /// <param name="height">Height of the screenshot</param>
        /// <param name="includeFileExtension">To generate the file extension behind the name</param>
        /// <param name="includePlatform">Include the current platform in the name</param>
        /// <param name="prefix">Custom string added to the beginning of the name</param>
        /// <param name="suffix">Custom string added to the end of the name (but before the file extension)</param>
        /// <returns>string file name</returns>
        private string GetFileName(int width, int height, bool includeFileExtension = true, bool includePlatform = true, string prefix = "", string suffix = "")
        {
            if(includeFileExtension)
            {
                return string.Format("{0}{1}_{2}_{3}x{4}{5}.{6}", prefix, includePlatform ? CurrentPlatform.displayName : "", System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-ff"), width, height, suffix, fileExtension);
            }
            else
            {
                return string.Format("{0}{1}_{2}_{3}x{4}{5}", prefix, includePlatform ? CurrentPlatform.displayName : "", System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-ff"), width, height, suffix);
            }
        }

        /// <summary>
        /// Get resolution for camera and screenshot
        /// </summary>
        /// <param name="options">The screenshot options</param>
        /// <param name="targetCamera">The camera the resolution is for</param>
        /// <returns>Resolution in pixels</returns>
        private Vector2Int GetCameraAndScreenshotResolution(Options options, Camera targetCamera)
        {
            if(options.resolution != Vector2Int.zero && options.resolution != new Vector2Int(-1, -1))
            {
                return options.resolution;
            }
            else if(Instance.CurrentPlatform.CurrentResolution.X == -1 && Instance.CurrentPlatform.CurrentResolution.Y == -1 || options.useCameraResolution)
            {
                return new Vector2Int(targetCamera.pixelWidth, targetCamera.pixelHeight);
            }
            else return Instance.CurrentPlatform.CurrentResolution.size;
        }

        /// <summary>
        /// Set the camera resolution
        /// </summary>
        /// <param name="targetCamera">The camera it is for</param>
        /// <param name="resolution">The resolution the camera needs to be</param>
        private void SetCameraResolution(Camera targetCamera, Vector2Int resolution)
        {
            targetCamera.pixelRect = new Rect(0, 0, resolution.x, resolution.y);

            // Set camera rect
            float targetaspect = (float)resolution.x / (float)resolution.y;
            // determine the game window's current aspect ratio
            float windowaspect = (float)resolution.x / (float)resolution.y;
            // current viewport height should be scaled by this amount
            float scaleheight = windowaspect / targetaspect; //TODO this is always 1 so why is this needed? (copy pasted this stuff off the web)
            // if scaled height is less than current height, add letterbox
            if(scaleheight < 1.0f) // This never gets called
            {
                Rect rect = targetCamera.rect;
                rect.width = 1.0f;
                rect.height = scaleheight;
                rect.x = 0;
                rect.y = (1.0f - scaleheight) / 2.0f;
                targetCamera.rect = rect;
            }
            else // add container box
            {
                float scalewidth = 1.0f / scaleheight;
                Rect rect = targetCamera.rect;
                rect.width = scalewidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scalewidth) / 2.0f;
                rect.y = 0;
                targetCamera.rect = rect;
            }
        }

        /// <summary>
        /// Get the n index of a string from char
        /// </summary>
        /// <param name="s">string</param>
        /// <param name="t">the char</param>
        /// <param name="n">which occurrence index</param>
        /// <returns></returns>
        private int GetNthIndex(string s, char t, int n)
        {
            int count = 0;
            for(int i = 0; i < s.Length; i++)
            {
                if(s[i] == t)
                {
                    count++;
                    if(count == n)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Load the editorPref values
        /// </summary>
        private void Load()
        {
            foldoutQuickAccess = EditorPrefs.GetBool(EDITORPREF_PREFIX + "foldoutQuickAccess", true);
            foldoutPlatforms = EditorPrefs.GetBool(EDITORPREF_PREFIX + "foldoutPlatforms", false);
            foldoutResolutions = EditorPrefs.GetBool(EDITORPREF_PREFIX + "foldoutResolutions", false);
            foldoutEditorWindows = EditorPrefs.GetBool(EDITORPREF_PREFIX + "foldoutEditorWindows", false);
            foldoutSettings = EditorPrefs.GetBool(EDITORPREF_PREFIX + "foldoutSettings", false);

            currentPlatformIndex = EditorPrefs.GetInt(EDITORPREF_PREFIX + "currentPlatformIndex", 0);
            currentEditorWindowIndex = EditorPrefs.GetInt(EDITORPREF_PREFIX + "currentEditorWindowIndex", 0);

            // Load platform resolutions
            string[] platformNames = EditorPrefs.GetString(EDITORPREF_PREFIX + "platformsDisplayNames", "").Split('#');
            for(int i = 0; i < platformNames.Length; i++)
            {
                // Get the platform name and amount of resolutions saved
                if(string.IsNullOrEmpty(platformNames[i])) continue;
                string displayName = platformNames[i].Substring(0, platformNames[i].IndexOf('<'));
                int rAmount = 0;
                int pFrom = platformNames[i].IndexOf('<') + 1;
                int pTo = platformNames[i].LastIndexOf('>');
                string pString = platformNames[i].Substring(pFrom, pTo - pFrom);
                int.TryParse(pString, out rAmount);
                if(rAmount < 1) continue;

                // Get all the resolutions
                List<Resolution> resolutions = new List<Resolution>();
                for(int j = 0; j < rAmount; j++)
                {
                    string ress = EditorPrefs.GetString(EDITORPREF_PREFIX + displayName + "_" + j);
                    // Get x / y
                    int x = -1, y = -1;
                    int startIndex = GetNthIndex(ress, '<', 1) + 1;
                    int.TryParse(ress.Substring(startIndex, ress.IndexOf('>', startIndex) - startIndex), out x);
                    startIndex = GetNthIndex(ress, '<', 2) + 1;
                    int.TryParse(ress.Substring(startIndex, ress.IndexOf('>', startIndex) - startIndex), out y);
                    // Get bool
                    bool include = true;
                    startIndex = GetNthIndex(ress, '<', 3) + 1;
                    bool.TryParse(ress.Substring(startIndex, ress.IndexOf('>', startIndex) - startIndex), out include);
                    // Get name
                    string resName = ress.Substring(ress.LastIndexOf('<'));

                    resolutions.Add(new Resolution(x, y, displayName, include));
                }

                // Check if a loaded platform has the same display name, if so replace default hc values
                platforms.First(x => x.displayName == displayName).resolutions = new List<Resolution>(resolutions);
                platforms.First(x => x.displayName == displayName).Init();
            }

            fileDirectory = EditorPrefs.GetString(EDITORPREF_PREFIX + "fileDirectory", "");
            if(string.IsNullOrEmpty(fileDirectory)) fileDirectory = fileDirectoryDefault;
            fileExtension = (FileExtension)EditorPrefs.GetInt(EDITORPREF_PREFIX + "fileExtension", 0);
            textureFormat = (TextureFormat)EditorPrefs.GetInt(EDITORPREF_PREFIX + "textureFormat", 4);
            showLoadingBar = EditorPrefs.GetBool(EDITORPREF_PREFIX + "showLoadingBar", true);
            showLoadingBar = EditorPrefs.GetBool(EDITORPREF_PREFIX + "showScreenshotMessage", true);
        }

        /// <summary>
        /// Save the editorPref values
        /// </summary>
        private void Save()
        {
            EditorPrefs.SetBool(EDITORPREF_PREFIX + "foldoutQuickAccess", foldoutQuickAccess);
            EditorPrefs.SetBool(EDITORPREF_PREFIX + "foldoutPlatforms", foldoutPlatforms);
            EditorPrefs.SetBool(EDITORPREF_PREFIX + "foldoutResolutions", foldoutResolutions);
            EditorPrefs.SetBool(EDITORPREF_PREFIX + "foldoutEditorWindows", foldoutEditorWindows);
            EditorPrefs.SetBool(EDITORPREF_PREFIX + "foldoutSettings", foldoutSettings);

            EditorPrefs.SetInt(EDITORPREF_PREFIX + "currentPlatformIndex", currentPlatformIndex);
            EditorPrefs.SetInt(EDITORPREF_PREFIX + "currentEditorWindowIndex", currentEditorWindowIndex);

            // Save platforms resolutions
            string s = "";
            foreach(Platform platform in platforms)
            {
                // Store name and amount of resolutions saved (SLIDDES_Unity_Screenshot_Standalone<12>#)
                s += string.Format("{0}<{1}>#", platform.displayName, platform.resolutions.Count);
                for(int i = 0; i < platform.resolutions.Count; i++)
                {
                    // Store each resolution under platform name (SLIDDES_Unity_Screenshot_Standalone_x<-1>y<-1>b<true>s<NameForResolution>)
                    EditorPrefs.SetString(EDITORPREF_PREFIX + platform.displayName + "_" + i, string.Format("x<{0}>y<{1}>b<{2}>s<{3}>", platform.resolutions[i].X, platform.resolutions[i].Y, platform.resolutions[i].include, platform.resolutions[i].displayName));
                }
            }
            EditorPrefs.SetString(EDITORPREF_PREFIX + "platformsDisplayNames", s);

            EditorPrefs.SetString(EDITORPREF_PREFIX + "fileDirectory", fileDirectory);
            EditorPrefs.SetInt(EDITORPREF_PREFIX + "fileExtension", (int)fileExtension);
            EditorPrefs.SetInt(EDITORPREF_PREFIX + "textureFormat", (int)textureFormat);
            EditorPrefs.SetBool(EDITORPREF_PREFIX + "showLoadingBar", showLoadingBar);
            EditorPrefs.SetBool(EDITORPREF_PREFIX + "showScreenshotMessage", showScreenshotMessage);
            EditorPrefs.SetBool(EDITORPREF_PREFIX + "resetOnOpen", resetOnOpen);
        }
        
        /// <summary>
        /// The options when taking a screenshot
        /// </summary>
        public class Options
        {
            /// <summary>
            /// If the screenshot resolution is equal to that of the camera resolution
            /// </summary>
            public bool useCameraResolution = false;
            /// <summary>
            /// If the screenshot should include UI/Gizmos
            /// </summary>
            public bool includeUI;
            /// <summary>
            /// What camera to take the screenshot from. 0 = scene view, 1 = game view, 2 = custom
            /// </summary>
            public int cameraIndex = 2;
            /// <summary>
            /// The resolution of the screenshot
            /// </summary>
            public Vector2Int resolution;

            public Options() { }

            public Options(Options copy)
            {
                useCameraResolution = copy.useCameraResolution;
                includeUI = copy.includeUI;
                cameraIndex = copy.cameraIndex;
                resolution = copy.resolution;
            }

            /// <summary>
            /// The options for a screenshot
            /// </summary>
            /// <param name="cameraIndex">What camera to take the screenshot from. 0 = scene view, 1 = game view, 2 = custom</param>
            /// <param name="includeUI">Should the screenshot include UI/Gizmos</param>
            public Options(int cameraIndex, bool includeUI = false)
            {
                this.cameraIndex = cameraIndex;
                this.includeUI = includeUI;
            }

            /// <summary>
            /// The options for a screenshot
            /// </summary>
            /// <param name="cameraIndex">What camera to take the screenshot from. 0 = scene view, 1 = game view, 2 = custom</param>
            /// <param name="useCameraResolution">Use the camera current resolution</param>
            /// <param name="includeUI">Should the screenshot include UI/Gizmos</param>
            public Options(int cameraIndex, bool useCameraResolution, bool includeUI)
            {
                this.cameraIndex = cameraIndex;
                this.useCameraResolution = useCameraResolution;
                this.includeUI = includeUI;
            }

            /// <summary>
            /// The options for a screenshot
            /// </summary>
            /// <param name="cameraIndex">What camera to take the screenshot from. 0 = scene view, 1 = game view, 2 = custom</param>
            /// <param name="resolution">The resolution of the screenshot</param>
            /// /// <param name="includeUI">Should the screenshot include UI/Gizmos</param>
            public Options(int cameraIndex, Vector2Int resolution, bool includeUI = false)
            {
                this.cameraIndex = cameraIndex;
                this.resolution = resolution;
                this.includeUI = includeUI;
            }
        }
    }
}