using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
        private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        if (image != null && image.enabled)
        {
            image.GetComponent<Button>().onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        image.enabled = false;
    }

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

    public void Leaderboard()
    {
        SceneManager.LoadScene("Leaderboard");
    }

    public void ReturnToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
