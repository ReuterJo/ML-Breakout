using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class GameManager : MonoBehaviour
{
    [Tooltip("The UI Controller")]
    public UIController uiController;

    [Tooltip("The paddle ball")]
    public BallBehavior ballBehavior;

    [Tooltip("The agent paddle")]
    public AgentBehavior agentBehavior;

    [Tooltip("The level generator")]
    public LevelGenerator levelGenerator;

    private int score;
    private int maxScore;
    private int brickValue = 10;
    private int lives;
    private Rigidbody2D ballRb;

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
        agentBehavior.BrickDestoryed();
    }

    /// <summary>
    /// Called when a life is lost
    /// </summary>
    public void loseLife()
    {
        lives--;
        agentBehavior.BallLost();
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    public void StartGame()
    {
        // Set the game state to preparing
        State = GameState.Preparing;

        // Generate level
        levelGenerator.GenerateLevel();
        maxScore = levelGenerator.size.x * levelGenerator.size.y * brickValue;

        // Update lives and score
        score = 0;
        lives = 5;
        uiController.ShowLives(lives.ToString());
        uiController.ShowScore(score.ToString());

        // Reset paddle ball
        ballBehavior.Reset();
        agentBehavior.Reset();

        // Unfreeze player and ball
        ballBehavior.Unfreeze();
        agentBehavior.Unfreeze();

        // Set the game state to playing
        State = GameState.Playing;
    }

    private void Awake()
    {
        ballRb = ballBehavior.GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Ends the game
    /// </summary>
    private void EndGame()
    {
        // Set the game state to game over
        State = GameState.Gameover;

        // Deconstruct level


        // Freeze player and ball
        ballBehavior.Freeze();
        agentBehavior.Freeze();
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        if (State == GameState.Playing)
        {
            // Reward agent for ball moving
            if (ballRb.velocity.magnitude > 0) agentBehavior.BallMoving();

            // Update score and lives
            uiController.ShowLives(lives.ToString());
            uiController.ShowScore(score.ToString());

            // Check to see if the player has lost all lives or reached max score
            if (lives == 0 || score == maxScore)
            {
                EndGame();
                agentBehavior.EndTrainingEpisode();   
            }
        }
    }
}
