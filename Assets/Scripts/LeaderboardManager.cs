using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    public string leaderboardJsonFilePath = "Assets/Files/Leaderboard.json";
    public TextMeshProUGUI leaderboardHeader;
    public TextMeshProUGUI leaderboardText;
    public TMP_InputField playerNameInput;
    public Button resetButton;
    private List<PlayerScore> leaderboard;
    private int minScore;
    

    // create PlayerScore class to store and serialize scores
    [System.Serializable]
    public class PlayerScore
    {
        public string name;
        public int score;
        public string date;
    }

    // create Leaderboard class to store and serialize score list
    [System.Serializable]
    public class Leaderboard
    {
        public List<PlayerScore> scores;
    }

    void Start()
    {
        this.LoadLeaderboard();
        this.PopulateLeaderBoard();
        this.playerNameInput.gameObject.SetActive(false);
    }

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

    public void AddScore(int score)
    {
        // Add score if it is above the minimum or if less than 8 total scores
        if (score > this.minScore || this.leaderboard.Count < 8)
        {
            // Get player name
            this.playerNameInput.gameObject.SetActive(true);
            //playerNameInput.onEndEdit.AddListener();
            this.playerNameInput.gameObject.SetActive(false);
            if (this.playerNameInput.text == "")
            {
                this.playerNameInput.text = "Unknown";
            }
            // Save and add the score
            PlayerScore newScore = new PlayerScore { name = playerNameInput.text, score = score, date = DateTime.Now.ToString("MM-dd-yyyy") };
            this.leaderboard.Add(newScore);
            // Sort the scores
            this.leaderboard.Sort((x, y) => y.score.CompareTo(x.score));
            // If there are more than 10 scores, delete the lowest
            if (this.leaderboard.Count > 8)
            {
                this.leaderboard.RemoveAt(leaderboard.Count - 1);
            }
            // Save and display leaderboard
            this.SaveLeaderboard();
        }
        this.LoadLeaderboard();
        this.PopulateLeaderBoard();
    }

    public void SaveLeaderboard()
    {
        Leaderboard leaderboardData = new Leaderboard { scores = leaderboard };
        string json = JsonUtility.ToJson(leaderboardData, true);
        File.WriteAllText(this.leaderboardJsonFilePath, json);
    }
}