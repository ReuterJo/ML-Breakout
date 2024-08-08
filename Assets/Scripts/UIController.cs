using System.Collections;
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
    [Tooltip("The level up text")]
    public TextMeshProUGUI levelUpText;
    [Tooltip("The pause canvas")]
    public TextMeshProUGUI pauseText;
    public Canvas pauseCanvas;


    /// <summary>
    /// Sets the screen position of the UI elements
    /// </summary>
    /// <param name="xShift">The horizontal shift</param>
    public void SetScreenPosition(float xShift)
    {
        this.livesText.transform.position += new Vector3(xShift, 0, 0);
        this.scoreText.transform.position += new Vector3(xShift, 0, 0);
        this.levelText.transform.position += new Vector3(xShift, 0, 0);
        this.levelUpText.transform.position += new Vector3(xShift, 0, 0);
    }

    /// <summary>
    /// Show the lives remaining text
    /// </summary>
    /// <param name="text">The text to be displayed</param>
    public void ShowLives(string text)
    {
        this.livesText.text = text;
        this.livesText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Show the level up text
    /// </summary>
    /// <param name="text">The text to be displayed</param>
    /// <param name="seconds">The duration for which the text is visible</param>
    /// <returns></returns>
    public IEnumerator ShowLevelUpText(string text, float seconds)
    {
        this.levelUpText.text = text;
        this.levelUpText.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        this.levelUpText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Show the level up text
    /// </summary>
    /// <param name="text">The text to be displayed</param>
    /// <param name="seconds">The duration for which the text is visible</param>
    /// <returns></returns>
    public void ShowEndGameText(string text)
    {
        this.levelUpText.text = text;
        this.levelUpText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Display the countdown timer
    /// </summary>
    /// <param name="playerType">The type of player</param>
    public async void CountdownTimer(PlayerType playerType)
    {
        int counter = 5;
        string player = "Player Game\nStarting In:\n";
        if (playerType == PlayerType.Agent) player = "Agent Game \nStarting In:\n";
        this.levelUpText.gameObject.SetActive(true);
        while (counter != 0)
        {
            this.levelUpText.text = player + counter;
            await Task.Delay(1000);
            counter--;
        }
        this.levelUpText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Show the pause canvas
    /// </summary>
    public void ShowPauseCanvas()
    {
        this.pauseCanvas.enabled = true;
    }

    /// <summary>
    /// Hide the pause canvas
    /// </summary>
    public void HidePauseCanvas()
    {
        this.pauseCanvas.enabled = false;
    }

    /// <summary>
    /// Display the game over text
    /// </summary>
    /// <param name="text">The text to be displayed</param>
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

    /// <summary>
    /// Show the level text
    /// </summary>
    /// <param name="text">The text to be displayed</param>
    public void ShowLevel(string text)
    {
        this.levelText.text = text;
        this.levelText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hide the level text
    /// </summary>
    public void HideLevel()
    {
        this.levelText.gameObject.SetActive(false);
    }

    void Start()
    {
        this.levelUpText.gameObject.SetActive(true);
        this.pauseCanvas.enabled = false;
        this.pauseText.text = "Game Paused";
    }
}
