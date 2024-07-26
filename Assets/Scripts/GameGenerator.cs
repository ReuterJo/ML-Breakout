using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGenerator : MonoBehaviour
{

    public GameObject gamePrefab; // Assign this in the Unity Inspector 
    public bool multi_level = true;            // use to change single or multi-level game
    public bool debug = false;
    public GameMode gameMode;
    private string model_path = "";
    public Difficulty difficulty;
    private bool training_mode = false;
    private GameObject game1;
    private GameManager gameManager1;
    private GameObject game2;
    private GameManager gameManager2;


    void Start()
    {
        this.SetAgentModel();
        this.GenerateGame();
    }

    void SetAgentModel()
    {
        switch (this.difficulty)
        {
            case Difficulty.Beginner:
                this.model_path = "NNModels/AgentBeginner";
                break;
            case Difficulty.Intermediate:
                this.model_path = "NNModels/AgentIntermediate";
                break;
            case Difficulty.Advanced:
                this.model_path = "NNModels/AgentAdvanced";
                break;
        }
    }

    void GenerateGame()
    {
        this.game1 = Instantiate(gamePrefab);
        this.gameManager1 = game1.GetComponentInChildren<GameManager>();
        switch (this.gameMode)
        {
            case GameMode.Training:
                this.TrainingGame();
                break;
            case GameMode.Single:
                this.SingleGame();
                break;
            case GameMode.Double:
                this.DoubleGame();
                break;
            default:
                Debug.Log("Error in gameMode selection.");
                break;
        }
    }

    void TrainingGame()
    {
        this.game1.name = "AgentGame";
        this.gameManager1.name = "AgentManager";
        this.training_mode = true;
        this.gameManager1.Configure(this.multi_level, 
                                this.training_mode, 
                                this.debug, 
                                PlayerType.Agent, 
                                null,
                                this.model_path);

    }


    void SingleGame()
    {
        this.game1.name = "PlayerGame";
        this.gameManager1.name = "PlayerManager";
        this.gameManager1.Configure(this.multi_level, 
                                this.training_mode, 
                                this.debug, 
                                PlayerType.Single, 
                                null,
                                null);
    }

    void DoubleGame()
    {
        this.game1.name = "PlayerGame";
        this.gameManager1.name = "PlayerManager";

        // Create the agent GameManager instance
        this.game2 = Instantiate(gamePrefab);
        this.game2.name = "AgentGame";
        gameManager2 = this.game2.GetComponentInChildren<GameManager>();
        this.gameManager2.name = "AgentManager";


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
