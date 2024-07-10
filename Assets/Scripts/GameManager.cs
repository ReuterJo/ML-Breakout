using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Tooltip("The UI Controller")]
    public UIController uiController;

    [Tooltip("The paddle ball")]
    public BallBehavior ballBehavior;

    [Tooltip("The player paddle")]
    public PlayerMovement playerMovement;

    [Tooltip("The level generator")]
    public LevelGenerator levelGenerator;

    private int score;
    private int maxScore;
    private int brickValue = 10;
    private int lives;

    /// <summary>
    /// All possible game states
    /// </summary>
    public enum GameState
    {
        Default,
        Preparing,
        Playing,
        Gameover
    }

    /// <summary>
    /// The current game state
    /// </summary>
    public GameState State { get; private set; } = GameState.Default;

    /// <summary>
    /// Called when a brick is destroyed
    /// </summary>
    public void scoreBrick()
    {   
        score += brickValue;
    }

    /// <summary>
    /// Called when a life is lost
    /// </summary>
    public void loseLife()
    {
        lives--;
    }

    /// <summary>
    /// Called before the game starts
    /// </summary>
    private void Awake()
    {
        levelGenerator = GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>();

        // Generate level
        levelGenerator.GenerateLevel();

        maxScore = levelGenerator.size.x * levelGenerator.size.y * brickValue;
    }

    /// <summary>
    /// Called when the game starts
    /// </summary>
    private void Start()
    {
        StartGame();
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    private void StartGame()
    {
        // Set the game state to preparing
        State = GameState.Preparing;

        // Update lives and score
        score = 0;
        lives = 5;
        uiController.ShowLives(lives.ToString());
        uiController.ShowScore(score.ToString());

        // Reset paddle ball
        ballBehavior.Reset();

        // Set the game state to playing
        State = GameState.Playing;
    }

    /// <summary>
    /// Ends the game
    /// </summary>
    private void EndGame()
    {
        // Set the game state to game over
        State = GameState.Gameover;

        // Reset the ball
        ballBehavior.Reset();

        // Freeze player and ball
        ballBehavior.Freeze();
        playerMovement.Freeze();
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        if (State == GameState.Playing)
        {
            // Update score and lives
            uiController.ShowLives(lives.ToString());
            uiController.ShowScore(score.ToString());

            // Check to see if the player has lost all lives or reached max score
            if (lives == 0 || score == maxScore)
            {
                EndGame();
            }
        }
        else if (State == GameState.Preparing || State == GameState.Gameover)
        {

        }
        else
        {

        }
    }
}
