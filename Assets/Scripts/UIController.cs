using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [Tooltip("The lives text")]
    public TextMeshProUGUI livesText;

    [Tooltip("The score text")]
    public TextMeshProUGUI scoreText;

    [Tooltip("The level text")]
    public TextMeshProUGUI levelText;

    /// <summary>
    /// Shows lives text
    /// </summary>
    /// <param name="text">Text of lives</param>
    public void ShowLives(string text)
    {
        livesText.text = text;
        livesText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the lives
    /// </summary>
    public void HideLives()
    {
        livesText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Shows the score
    /// </summary>
    /// <param name="text">Text of score</param>
    public void ShowScore(string text)
    {
        scoreText.text = text;
        scoreText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the score
    /// </summary>
    public void HideScore()
    {
        scoreText.gameObject.SetActive(false);
    }

    public void ShowLevel(string text)
    {
        levelText.text = text;
        levelText.gameObject.SetActive(true);
    }

    public void HideLevel()
    {
        levelText.gameObject.SetActive(false);
    }
}
