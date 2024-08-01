using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void SinglePlayer()
    {
        SceneManager.LoadScene("SinglePlayer");
    }

    public void Beginner()
    {
        SceneManager.LoadScene("PlayerAgentScene");
    }

    public void Intermediate()
    {
        SceneManager.LoadScene("PlayerAgentScene");
    }

    public void Advanced()
    {
        SceneManager.LoadScene("PlayerAgentScene");
    }

    public void Training()
    {
        SceneManager.LoadScene("Training");
    }

    public void About()
    {
        SceneManager.LoadScene("About");
    }

    public void Leaderboard()
    {
        SceneManager.LoadScene("Leaderboard");
    }

    public void ReturnToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
