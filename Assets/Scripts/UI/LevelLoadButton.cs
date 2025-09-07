using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is meant to be used on buttons as a quick easy way to load levels (scenes)
/// </summary>
public class LevelLoadButton : MonoBehaviour
{

    [Tooltip("Reset the score when this button loads the level?")]
    public bool resetScore = false;
    /// <summary>
    /// Loads a level according to the name provided
    /// </summary>
    /// <param name="levelToLoadName">The name of the level to load</param>
    /// <param name="resetScore">Whether to reset the player's score before loading</param>
    public void LoadLevelByName(string levelToLoadName)
    {
        Time.timeScale = 1;

        if (resetScore && GameManager.instance != null)
        {
            GameManager.ResetScore();
        }

        SceneManager.LoadScene(levelToLoadName);
    }
}
