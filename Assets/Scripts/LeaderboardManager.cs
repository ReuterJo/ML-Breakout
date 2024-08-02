using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class LeaderboardManager : MonoBehaviour
{
    public string leaderboardJsonFilePath = "Assets/Files/Leaderboard.json";
    public TextMeshProUGUI leaderboardHeader;
    public TextMeshProUGUI leaderboardText;
    public TMP_InputField playerNameInput;
    public Button resetButton;
    public Button submitButton;

    private List<PlayerScore> leaderboard;
    private int minScore;
    private int tempScore;

    public Canvas leaderboardCanvas;
    
    /// <summary>
    /// Create PlayerScore class to store and serialize scores
    /// </summary>
    [System.Serializable]
    public class PlayerScore
    {
        public string name;
        public int score;
        public string date;
    }

    /// <summary>
    /// Create Leaderboard class to store and serialize score list
    /// </summary>
    [System.Serializable]
    public class Leaderboard
    {
        public List<PlayerScore> scores;
    }

    /// <summary>
    /// Load existing leadboard from file
    /// </summary>
    public void LoadLeaderboard()
    {
        if (File.Exists(this.leaderboardJsonFilePath))
        {
            string json = File.ReadAllText(this.leaderboardJsonFilePath);
            Leaderboard loadedData = JsonUtility.FromJson<Leaderboard>(json);
            this.leaderboard = loadedData.scores;

            // Initialize min and max scores with the first score
            if (this.leaderboard != null && this.leaderboard.Count > 0)
            {
                this.minScore = this.leaderboard[0].score;

                foreach (PlayerScore scoreEntry in this.leaderboard)
                {
                    if (scoreEntry.score < this.minScore)
                        this.minScore = scoreEntry.score;
                }
            }
        }
        else
        {
            this.minScore = 0; // Reset minScore to lowest possible integer value if leaderboard is empty
            this.leaderboard = new List<PlayerScore>();
        }
    }

    /// <summary>
    /// Add a score to the leaderboard
    /// </summary>
    /// <param name="score">The score to be added</param>
    public void AddScore(int score)
    {
        this.leaderboardCanvas.enabled = true;
        // Add score if it is above the minimum or if less than 8 total scores
        if (score > this.minScore || this.leaderboard.Count < 8)
        {
            this.playerNameInput.gameObject.SetActive(true);
            this.submitButton.gameObject.SetActive(true);
            this.tempScore = score;
        }
        this.LoadLeaderboard();
        this.PopulateLeaderBoard();
    }

    /// <summary>
    /// Save the leaderboard to file
    /// </summary>
    public void SaveLeaderboard()
    {
        Leaderboard leaderboardData = new Leaderboard { scores = leaderboard };
        string json = JsonUtility.ToJson(leaderboardData, true);
        File.WriteAllText(this.leaderboardJsonFilePath, json);
    }

    /// <summary>
    /// Populate the leadboard text
    /// </summary>
    private void PopulateLeaderBoard()
    {
        this.leaderboardText.text = "";

        int count = 1;
        foreach (var playerScore in leaderboard)
        {
            this.leaderboardText.text += count + ". Player: " + playerScore.name + "  Score: " + playerScore.score + "  Date: " + playerScore.date + "\n";
            count += 1;
        }
    }

    /// <summary>
    /// Accept a player name for the leadboard entry
    /// </summary>
    private void SetPlayerName()
    {
        if (playerNameInput.text == "") playerNameInput.text = "Unknown";
        // Save and add the score
        PlayerScore newScore = new PlayerScore { name = playerNameInput.text, score = this.tempScore, date = DateTime.Now.ToString("MM-dd-yyyy") };
        this.leaderboard.Add(newScore);
        // Sort the scores
        this.leaderboard.Sort((x, y) => y.score.CompareTo(x.score));
        // If there are more than 10 scores, delete the lowest
        if (this.leaderboard.Count > 8)
        {
            this.leaderboard.RemoveAt(leaderboard.Count - 1);
        }
        this.tempScore = 0;
        this.playerNameInput.gameObject.SetActive(false);
        this.submitButton.gameObject.SetActive(false);
        // Save and display leaderboard
        this.SaveLeaderboard();
        this.LoadLeaderboard();
        this.PopulateLeaderBoard();
    }

    void Start()
    {
        this.LoadLeaderboard();
        this.PopulateLeaderBoard();
        this.playerNameInput.gameObject.SetActive(false);
        this.submitButton.gameObject.SetActive(false);
        submitButton.onClick.AddListener(() => {
            SetPlayerName();
        });
        this.leaderboardCanvas.enabled = true;
    }

    public void HideLeaderboard()
    {
        this.leaderboardCanvas.enabled = false;
    }

        public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}