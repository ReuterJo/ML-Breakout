using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class GameManager : MonoBehaviour
{
    [Tooltip("Sets the UI Controller script")]
    public UIController uiController;

    [Tooltip("Sets the BallBehavior script")]
    public BallBehavior ballBehavior;

    [Tooltip("Sets the AgentBehavior script")]
    public AgentBehavior agentBehavior;

    [Tooltip("Sets the LevelGenerator script")]
    public LevelGenerator levelGenerator;

    private int score = 0;
    private int brickValue = 10;
    private int bricksRemaining;
    private int lives = 5;
    private Rigidbody2D ballRb;
    private int level;
    private float levelStartTime;
    private bool multi_level = true;  // use to change single or multi-level game

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
        bricksRemaining -= 1;
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
        bricksRemaining = levelGenerator.ChangeLevel(1);
        if (!multi_level)
        {
            uiController.HideLevel();
        }
        else
        {
            level = 1;
            uiController.ShowLevel("Level " + level.ToString());
        }

        // Update lives and score
        uiController.ShowLives(lives.ToString() + " Lives");
        uiController.ShowScore("Score " + score.ToString());

        // Begin the level timer
        levelStartTime = Time.time;

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
            uiController.ShowLives(lives.ToString() + " Lives");
            uiController.ShowScore("Score " + score.ToString());

            // Check to see if the player has lost all lives
            if (lives == 0)
            {
                EndGame();
                agentBehavior.EndTrainingEpisode();   
            }

            // Single level game
            if (!multi_level)
            {
                if (bricksRemaining == 0)
                {
                    EndGame();
                    agentBehavior.EndTrainingEpisode();  
                }
            }
            // Multi-level game
            else
            {
                // finished game
                if (level == 5)
                {
                    EndGame();
                    agentBehavior.EndTrainingEpisode();  
                }
                else
                {
                    if ((level == 1 && bricksRemaining == 27) || bricksRemaining == 0) levelGenerator.ChangeLevel(level + 1);
                    uiController.ShowLevel("Level " + level.ToString());
                }
            }
        }
    }

    public void ChangeLevel(float pointMultiplier, int maxBonusPoints, float decreasePaddlePercent, float increaseBallVelocityPercent)
    {
        // change brickValue 
        brickValue = (int) (brickValue * pointMultiplier);

        // give level bonus points based on time to complete level

        float elapsed = Time.time - levelStartTime;
        int bonus = (int) (maxBonusPoints / elapsed);
        score += bonus;

        // change paddle size
        agentBehavior.ChangePaddleScale(decreasePaddlePercent);

        // change ball velocity
        ballBehavior.ChangeBallVelocity(increaseBallVelocityPercent);

        // change level
        level += 1;
    }

}
