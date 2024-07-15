using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class GameManager : MonoBehaviour
{
    // script variables
    [Tooltip("Sets the UI Controller script")]
    public UIController uiController;

    [Tooltip("Sets the BallBehavior script")]
    public BallBehavior ballBehavior;

    [Tooltip("Sets the AgentBehavior script")]
    public AgentBehavior agentBehavior;

    [Tooltip("Sets the LevelGenerator script")]
    public LevelGenerator levelGenerator;

    // game variables
    private int score = 0;
    private int brickValue = 10;
    private int bricksRemaining;
    private int lives = 5;
    private Rigidbody2D ballRb;
    private float levelStartTime;

    // configuration variables
    private bool multi_level = true;            // use to change single or multi-level game
    private bool training_mode = false;         // use to train the model vs play the game
    private int level = 1;                      // use to set starting level in multi-level mode

    // level variables
    private float levelPointMultiplier = 1.5f;
    private float levelPaddleSizeMultiplier = 0.9f;
    private float levelBonusMultiplier = 1.5f;
    private float levelBallVelocityMultiplier = 0.2f;

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
        uiController.ShowScore("Score " + score.ToString());
        bricksRemaining -= 1;
        agentBehavior.BrickDestoryed();
    }

    /// <summary>
    /// Called when a life is lost
    /// </summary>
    public void loseLife()
    {
        lives--;
        uiController.ShowLives(lives.ToString() + " Lives");
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

        if (!multi_level)
        {
            uiController.HideLevel();
            bricksRemaining = levelGenerator.ChangeLevel(level);
        }
        else
        {
            if (level == 1)
            {
                bricksRemaining = levelGenerator.ChangeLevel(level);
            }
            else
            {
                // decrement level by one to generate correct level
                level -= 1;
                ChangeLevel();
            }
            
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

            // Check to see if the player has lost all lives
            if (lives == 0)
            {
                if (!training_mode) EndGame();
                else agentBehavior.EndTrainingEpisode();   
            }

            // Single level game
            if (!multi_level)
            {
                // finished single-level game
                if (bricksRemaining == 0)
                {
                    if (!training_mode) EndGame();
                    else agentBehavior.EndTrainingEpisode();  
                }
            }
            // Multi-level game
            else
            {
                // finished multi-level game
                if (bricksRemaining == 0 && level == 5)
                {
                    if (!training_mode) EndGame();
                    else agentBehavior.EndTrainingEpisode();  
                }
                // move to the next level
                else
                {
                    if ((level == 1 && bricksRemaining == 27) || bricksRemaining == 0) ChangeLevel();
                }
            }
        }
    }

    public void ChangeLevel()
    {
        // increment level
        level += 1;

        // change brickValue 
        brickValue = (int) (brickValue * level * levelPointMultiplier);

        // give level bonus points based on time to complete level
        float elapsed = Time.time - levelStartTime;
        int bonus = (int) ((level * levelBonusMultiplier * 1000) / elapsed);
        score += bonus;

        // change paddle size
        agentBehavior.ChangePaddleScale(level * levelPaddleSizeMultiplier);

        // change ball velocity
        ballBehavior.ChangeBallVelocity(level * levelBallVelocityMultiplier);

        // generate blocks
        bricksRemaining = levelGenerator.ChangeLevel(level);

        // update level UI display
        uiController.ShowLevel("Level " + level.ToString());
    }

}
