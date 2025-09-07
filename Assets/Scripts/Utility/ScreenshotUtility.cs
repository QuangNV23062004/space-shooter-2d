/*
 *           ~~ Screenshot Utility ~~ 
 *  Takes a screenshot of the game window with its
 *  current resolution. Should work in the editor 
 *  or on any platform.
 *  
 *  Created by Brian Winn, Michigan State University
 *  Games for Entertainment and Learning (GEL) Lab
 * 
 *  Notes:
 *    - Images are stored in a Screenshots folder within the Unity project directory.
 * 
 *    - Images will be copied over if player prefs are reset!
 * 
 *    - If the resolution is 1024x768, and the scale factor
 *      is 2, the screenshot will be saved as 2048x1536.
 * 
 *    - The mouse is not captured in the screenshot.
 * 
 * Last Updated: September 7, 2025
 */

using UnityEngine;
using System.Collections;
using System.IO; // for Directory/File IO
using UnityEngine.InputSystem; // for Unity's new Input System

/// <summary>
/// Handles taking a screenshot of the game.
/// Supports single key or multi-key combinations (e.g., "ctrl,s").
/// </summary>
public class ScreenshotUtility : MonoBehaviour
{
    public static ScreenshotUtility screenShotUtility;

    #region Public Variables
    [Header("Settings")]
    [Tooltip("Should the screenshot utility run only in the editor.")]
    public bool runOnlyInEditor = true;

    [Tooltip("Keys for taking screenshot (comma-separated, e.g. 'ctrl,s' or just 'c').")]
    public string m_ScreenshotKeys = "c";

    [Tooltip("What is the scale factor for the screenshot. Standard is 1, 2x size is 2, etc..")]
    public int m_ScaleFactor = 1;

    [Tooltip("Include image size in filename.")]
    public bool includeImageSizeInFilename = true;
    #endregion

    #region Private Variables
    [Header("Private Variables")]
    [Tooltip("Use the Reset Counter contextual menu item to reset this.")]
    [SerializeField] private int m_ImageCount = 0;

    private Key[] screenshotKeys;
    #endregion

    #region Constants
    private const string ImageCntKey = "IMAGE_CNT";
    #endregion

    void Awake()
    {
        if (screenShotUtility != null)
        {
            Destroy(this.gameObject);
        }
        else if (runOnlyInEditor && !Application.isEditor)
        {
            Destroy(this.gameObject);
        }
        else
        {
            screenShotUtility = this.GetComponent<ScreenshotUtility>();
            DontDestroyOnLoad(gameObject);

            m_ImageCount = PlayerPrefs.GetInt(ImageCntKey);

            if (!Directory.Exists("Screenshots"))
            {
                Directory.CreateDirectory("Screenshots");
            }

            // Parse keys from string input
            string[] keys = m_ScreenshotKeys.Split(',');
            screenshotKeys = new Key[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                if (System.Enum.TryParse(keys[i].Trim(), true, out Key parsedKey))
                {
                    screenshotKeys[i] = parsedKey;
                }
                else
                {
                    Debug.LogWarning($"Invalid key: {keys[i]}");
                }
            }
        }
    }

    void Update()
    {
        if (screenshotKeys == null || screenshotKeys.Length == 0) return;

        // Check if *all* keys are currently pressed
        bool allPressed = true;
        foreach (var key in screenshotKeys)
        {
            if (Keyboard.current[key] == null || !Keyboard.current[key].isPressed)
            {
                allPressed = false;
                break;
            }
        }

        // Trigger only when the last key is pressed this frame while holding the others
        if (allPressed && Keyboard.current[screenshotKeys[screenshotKeys.Length - 1]].wasPressedThisFrame)
        {
            TakeScreenshot();
        }
    }

    [ContextMenu("Reset Counter")]
    public void ResetCounter()
    {
        m_ImageCount = 0;
        PlayerPrefs.SetInt(ImageCntKey, m_ImageCount);
    }

    public void TakeScreenshot()
    {
        PlayerPrefs.SetInt(ImageCntKey, ++m_ImageCount);

        int width = Screen.width * m_ScaleFactor;
        int height = Screen.height * m_ScaleFactor;

        string pathname = "Screenshots/Screenshot_";
        if (includeImageSizeInFilename)
        {
            pathname += width + "x" + height + "_";
        }
        pathname += m_ImageCount + ".png";

        ScreenCapture.CaptureScreenshot(pathname, m_ScaleFactor);
        Debug.Log("Screenshot captured at " + pathname);
    }
}
