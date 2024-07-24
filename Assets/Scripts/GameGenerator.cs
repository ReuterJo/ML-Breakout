using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGenerator : MonoBehaviour
{

    public GameObject gamePrefab; // Assign this in the Unity Inspector 
    public bool multi_level = true;            // use to change single or multi-level game
    public bool training_mode = false;         // use to train the model vs play the game
    public bool debug = false;
    public GameMode gameMode;
    private string model_path = "";
    public Difficulty difficulty;
    private GameObject game1;
    private GameManager gameManager1;
    private GameObject game2;
    private GameManager gameManager2;


    void Start()
    {
        InitializeGameManagers();
    }

    void InitializeGameManagers()
    {
        switch (this.difficulty)
        {
            case Difficulty.Beginner:
                this.model_path = "NNModels/AgentBehavior";
                break;
            case Difficulty.Intermediate:
                this.model_path = "NNModels/AgentBehavior";
                break;
            case Difficulty.Advanced:
                this.model_path = "NNModels/AgentBehavior";
                break;
        }

        if (this.gameMode == GameMode.Training)
        {
            this.game1 = Instantiate(gamePrefab);
            this.gameManager1 = game1.GetComponentInChildren<GameManager>();
            Debug.Log("Configuration Called.");
            this.gameManager1.Configure(this.multi_level, 
                                    this.training_mode, 
                                    this.debug, 
                                    PlayerType.Single, 
                                    null,
                                    this.model_path);

        }
        else if (this.gameMode == GameMode.Single)
        {
            this.game1 = Instantiate(gamePrefab);
            this.gameManager1 = game1.GetComponentInChildren<GameManager>();
            this.gameManager1.Configure(this.multi_level, 
                                    this.training_mode, 
                                    this.debug, 
                                    PlayerType.Single, 
                                    null,
                                    null);
        }
        else
        {
            // Create the player GameManager instance
            this.game1 = Instantiate(gamePrefab);
            gameManager1 = this.game1.GetComponentInChildren<GameManager>();

            // Create the agent GameManager instance
            this.game2 = Instantiate(gamePrefab);
            gameManager2 = this.game2.GetComponentInChildren<GameManager>();

            // Initialize the player gameManager settings
            this.gameManager1.Configure(this.multi_level, 
                                    this.training_mode, 
                                    this.debug, 
                                    PlayerType.Player, 
                                    gameManager2,
                                    null);

            // Initialize the agent gameManager settings
            this.gameManager2.Configure(this.multi_level, 
                            this.training_mode, 
                            this.debug, 
                            PlayerType.Agent, 
                            gameManager1,
                            this.model_path);
        }
    }
}
