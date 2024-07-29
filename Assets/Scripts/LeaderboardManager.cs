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
        LoadLeaderboard();
        PopulateLeaderBoard();
        playerNameInput.gameObject.SetActive(false);
    }

    public void LoadLeaderboard()
    {
        if (File.Exists(leaderboardJsonFilePath))
        {
            string json = File.ReadAllText(leaderboardJsonFilePath);
            Leaderboard loadedData = JsonUtility.FromJson<Leaderboard>(json);
            leaderboard = loadedData.scores;

            // Initialize min and max scores with the first score
            if (leaderboard != null && leaderboard.Count > 0)
            {
                this.minScore = leaderboard[0].score;

                foreach (PlayerScore scoreEntry in leaderboard)
                {
                    if (scoreEntry.score < this.minScore)
                        this.minScore = scoreEntry.score;
                }
            }
        }
        else
        {
            this.minScore = 0; // Reset minScore to lowest possible integer value if leaderboard is empty
            leaderboard = new List<PlayerScore>();
        }
        Debug.Log("Min Score: " + minScore);
    }

    private void PopulateLeaderBoard()
    {
        leaderboardText.text = "";
        int count = 1;
        foreach (var playerScore in leaderboard)
        {
            leaderboardText.text += count + ". Player: " + playerScore.name + "  Score: " + playerScore.score + "  Date: " + playerScore.date + "\n";
            count += 1;
        }
    }

    public void AddScore(int score)
    {
        // Add score if it is above the minimum or if less than 8 total scores
        if (score > this.minScore || leaderboard.Count < 8)
        {
            Debug.Log("High Score Entry");
            // Get player name
            playerNameInput.gameObject.SetActive(true);
            //playerNameInput.onEndEdit.AddListener();
            playerNameInput.gameObject.SetActive(false);
            if (playerNameInput.text == "")
            {
                playerNameInput.text = "Unknown";
            }
            // Save and add the score
            PlayerScore newScore = new PlayerScore { name = playerNameInput.text, score = score, date = DateTime.Now.ToString("MM-dd-yyyy") };
            leaderboard.Add(newScore);
            // Sort the scores
            leaderboard.Sort((x, y) => y.score.CompareTo(x.score));
            // If there are more than 10 scores, delete the lowest
            if (leaderboard.Count > 8)
            {
                leaderboard.RemoveAt(leaderboard.Count - 1);
            }
            // Save and display leaderboard
            SaveLeaderboard();
            LoadLeaderboard();
        }
    }

    public void SaveLeaderboard()
    {
        Leaderboard leaderboardData = new Leaderboard { scores = leaderboard };
        string json = JsonUtility.ToJson(leaderboardData, true);
        File.WriteAllText(leaderboardJsonFilePath, json);
    }
}