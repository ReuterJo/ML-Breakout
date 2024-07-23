using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGenerator : MonoBehaviour
{

    public GameObject gamePrefab; // Assign this in the Unity Inspector 
    public bool multi_level = true;            // use to change single or multi-level game
    public bool training_mode = false;         // use to train the model vs play the game
    public bool debug = false;
    public PlayerType playerType;
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
                this.model_path = "Assets/NNModels/AgentBehavior.onnx";
                break;
            case Difficulty.Intermediate:
                this.model_path = "Assets/NNModels/AgentBehavior.onnx";
                break;
            case Difficulty.Advanced:
                this.model_path = "Assets/NNModels/AgentBehavior.onnx";
                break;
        }

        if (training_mode)
        {
            this.game1 = Instantiate(gamePrefab);
            this.gameManager1 = game1.GetComponent<GameManager>();
            this.gameManager1.GameInitializer(this.multi_level, 
                                    this.training_mode, 
                                    this.debug, 
                                    PlayerType.Single, 
                                    null,
                                    this.model_path);

        }
        else if (playerType == PlayerType.Single)
        {
            this.game1 = Instantiate(gamePrefab);
            this.gameManager1 = game1.GetComponent<GameManager>();
            this.gameManager1.GameInitializer(this.multi_level, 
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
            this.gameManager1 = game1.GetComponent<GameManager>();

            // Create the agent GameManager instance
            this.game2 = Instantiate(gamePrefab);
            this.gameManager2 = game2.GetComponent<GameManager>();

            // Initialize the player gameManager settings
            this.gameManager1.GameInitializer(this.multi_level, 
                                    this.training_mode, 
                                    this.debug, 
                                    PlayerType.Player, 
                                    gameManager2,
                                    null);

            // Initialize the agent gameManager settings
            this.gameManager2.GameInitializer(this.multi_level, 
                            this.training_mode, 
                            this.debug, 
                            PlayerType.Agent, 
                            gameManager1,
                            this.model_path);
        }
    }
}
