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
    private string model_path;
    public Difficulty difficulty;

    void Start()
    {
        InitializeGameManagers();
    }

    void InitializeGameManagers()
    {
        this.model_path = "";
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
            GameObject game1 = Instantiate(gamePrefab);
            GameManager gameManager1 = game1.GetComponent<GameManager>();
            gameManager1.GameInitializer(this.multi_level, 
                                    this.training_mode, 
                                    this.debug, 
                                    PlayerType.Single, 
                                    ScreenPosition.Center, 
                                    null,
                                    this.model_path);

        }
        else if (playerType == PlayerType.Single)
        {
            GameObject game1 = Instantiate(gamePrefab);
            GameManager gameManager1 = game1.GetComponent<GameManager>();
            gameManager1.GameInitializer(this.multi_level, 
                                    this.training_mode, 
                                    this.debug, 
                                    PlayerType.Single, 
                                    ScreenPosition.Center, 
                                    null,
                                    null);
        }
        else
        {
            // Create the player GameManager instance
            GameObject game1 = Instantiate(gamePrefab);
            GameManager gameManager1 = game1.GetComponent<GameManager>();

            // Create the agent GameManager instance
            GameObject game2 = Instantiate(gamePrefab);
            GameManager gameManager2 = game2.GetComponent<GameManager>();

            // Initialize the player gameManager settings
            gameManager1.GameInitializer(this.multi_level, 
                                    this.training_mode, 
                                    this.debug, 
                                    PlayerType.Player, 
                                    ScreenPosition.Left, 
                                    gameManager2,
                                    null);

            // Initialize the agent gameManager settings
            gameManager2.GameInitializer(this.multi_level, 
                            this.training_mode, 
                            this.debug, 
                            PlayerType.Agent, 
                            ScreenPosition.Right, 
                            gameManager1,
                            this.model_path);
        }
    }
}
