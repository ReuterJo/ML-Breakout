using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void SinglePlayer()
    {
        SceneManager.LoadSceneAsync("SampleScene");
    }

        public void Beginner()
    {
        SceneManager.LoadSceneAsync("Beginner");
    }

        public void Intermediate()
    {
        SceneManager.LoadSceneAsync("Intermediate");
    }

        public void Advanced()
    {
        SceneManager.LoadSceneAsync("Advanced");
    }

        public void Training()
    {
        SceneManager.LoadSceneAsync("Training");
    }

}
