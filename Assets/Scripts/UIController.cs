using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class UIController : MonoBehaviour
{
    [Tooltip("The lives text")]
    public TextMeshProUGUI livesText;

    [Tooltip("The score text")]
    public TextMeshProUGUI scoreText;

    [Tooltip("The level text")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelUpText;
    public TextMeshProUGUI pauseText;
    public Canvas pauseCanvas;

    /// <summary>
    /// Shows lives text
    /// </summary>
    /// <param name="text">Text of lives</param>


    public void Start()
    {
        this.levelUpText.gameObject.SetActive(false);
        this.pauseCanvas.enabled = false;
        this.pauseText.text = "Game Paused";
    }

    public void ShowLives(string text)
    {
        this.livesText.text = text;
        this.livesText.gameObject.SetActive(true);
    }

    public void ShowLevelUpText(string text)
    {
        this.levelUpText.text = text;
        this.levelUpText.gameObject.SetActive(true);
    }

    public void HideLevelUpText()
    {
        this.levelUpText.gameObject.SetActive(false);
    }

    public void GameOverText(string text)
    {
        this.levelUpText.text = text;
        this.levelUpText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the lives
    /// </summary>
    public void HideLives()
    {
        this.livesText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Shows the score
    /// </summary>
    /// <param name="text">Text of score</param>
    public void ShowScore(string text)
    {
        this.scoreText.text = text;
        this.scoreText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the score
    /// </summary>
    public void HideScore()
    {
        this.scoreText.gameObject.SetActive(false);
    }

    public void ShowLevel(string text)
    {
        this.levelText.text = text;
        this.levelText.gameObject.SetActive(true);
    }

    public void HideLevel()
    {
        this.levelText.gameObject.SetActive(false);
    }

    public void ShowPauseCanvas()
    {
        this.pauseCanvas.enabled = true;
    }

    public void EndGame(string text)
    {
        this.pauseText.text = text;
        this.pauseCanvas.enabled = true;
    }

        public void HidePauseCanvas()
    {
        this.pauseCanvas.enabled = false;
    }
}
