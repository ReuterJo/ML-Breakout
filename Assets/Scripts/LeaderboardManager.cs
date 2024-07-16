using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public string jsonFilePath = "Assets/Files/Leaderboard.json";
    public TextMeshProUGUI leaderboardText;

    private List<PlayerScore> leaderboard;

    void Start()
    {
        LoadLeaderboard();
        DisplayLeaderboard();
    }

    public void LoadLeaderboard()
    {
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            Leaderboard loadedData = JsonUtility.FromJson<Leaderboard>(json);
            leaderboard = loadedData.scores;
        }
        else
        {
            leaderboard = new List<PlayerScore>();
        }
    }

    public void DisplayLeaderboard()
    {
        leaderboardText.text = "";
        foreach (var playerScore in leaderboard)
        {
            leaderboardText.text += "Player: " + playerScore.name + "  Score: " + playerScore.score + "  Date: " + playerScore.date + "\n";
        }
    }

    public void AddScore(string playerName, int score)
    {
        PlayerScore newScore = new PlayerScore { name = playerName, score = score, date = DateTime.Now.ToString("MM-dd-yyyy") };
        leaderboard.Add(newScore);
        leaderboard.Sort((x, y) => y.score.CompareTo(x.score));
        SaveLeaderboard();
        DisplayLeaderboard();
    }

    public void SaveLeaderboard()
    {
        Leaderboard leaderboardData = new Leaderboard { scores = leaderboard };
        string json = JsonUtility.ToJson(leaderboardData, true);
        File.WriteAllText(jsonFilePath, json);
    }
}

[System.Serializable]
public class PlayerScore
{
    public string name;
    public int score;
    public string date;
}

[System.Serializable]
public class Leaderboard
{
    public List<PlayerScore> scores;
}