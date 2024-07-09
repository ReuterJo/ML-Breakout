using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void SinglePlayer()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Beginner()
    {
        SceneManager.LoadScene("Beginner");
    }

    public void Intermediate()
    {
        SceneManager.LoadScene("Intermediate");
    }

    public void Advanced()
    {
        SceneManager.LoadScene("Advanced");
    }

    public void Training()
    {
        SceneManager.LoadScene("Training");
    }

    public void About()
    {
        SceneManager.LoadScene("About");
    }

    public void ReturnToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
