using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.SceneManagement;

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

    [Tooltip("Sets the LeaderboardManager script")]
    public LeaderboardManager leaderboardManager;

    [Tooltip("Sets the game manager object for this game")]
    public GameManager thisGame;

    [Tooltip("Sets the game manager object for the opponent game")]
    public GameManager opponentGame;

    [Tooltip("Sets the Screen Position")]
    public ScreenPosition screenPosition;

    [Tooltip("Sets the game to multilevel mode")]
    public bool multi_level = true;            // use to change single or multi-level game
    [Tooltip("Sets the game to training mode")]
    public bool training_mode = false;         // use to train the model vs play the game

    // game variables
    private int score;
    private int brickValue = 10;
    private int bricksRemaining;
    private int lives;
    private int level;
    private Rigidbody2D ballRb;
    private float levelStartTime;

    // configuration variables
    private int starting_level = 1;             // use to set starting level in multi-level mode

    // level variables
    private float levelPointMultiplier = 1.5f;
    private float levelPaddleSizeSubtractor = 0.2f;
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
        Paused,
        Gameover
    }

    /// <summary>
    /// The current game state
    /// </summary>
    public GameState State { get; private set; } = GameState.Default;

    void SetScreenPosition()
    {
        Vector3 newPosition = thisGame.transform.position;
        float screenWidth = Screen.width;
        float gameWidth = thisGame.GetComponent<Renderer>().bounds.size.x;

        switch (screenPosition)
        {
            case ScreenPosition.Left:
                newPosition.x = screenWidth * 0.25f;
                break;
            case ScreenPosition.Center:
                newPosition.x = screenWidth * 0.5f;
                break;
            case ScreenPosition.Right:
                newPosition.x = screenWidth * 0.75f;
                break;
        }

        thisGame.transform.position = newPosition;
    }

    public ScreenPosition GetScreenPosition()
    {
        return this.screenPosition;
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    public void StartGame()
    {
        // Set the game state to preparing
        State = GameState.Preparing;
        level = starting_level;
        lives = 5;
        score = 0;
        leaderboardManager.HideLeaderboard();

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
                uiController.ShowLevelUpText("Level " + level.ToString());
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

    private void PauseGame()
    {
        State = GameState.Paused;
        ballBehavior.Freeze();
        agentBehavior.Freeze();
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

        // Check if the leaderboard needs to be updated
        leaderboardManager.AddScore(score);
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
                    if ((level == 1 && bricksRemaining == 27) || bricksRemaining == 0) 
                    {
                        ChangeLevel();
                    }
                }
            }
        }
    }

    public void ChangeLevel()
    {
        Debug.Log("Change to Level " + level);
        // increment level
        level += 1;

        // change brickValue 
        brickValue = (int) (brickValue * level * levelPointMultiplier);

        // give level bonus points based on time to complete level
        float elapsed = Time.time - levelStartTime;
        int bonus = (int) ((level * levelBonusMultiplier * 1000) / elapsed);
        score += bonus;
        levelStartTime = 0;

        // change paddle size
        agentBehavior.ChangePaddleScale(level * levelPaddleSizeSubtractor);

        // change ball velocity
        ballBehavior.ChangeBallVelocity(level * levelBallVelocityMultiplier);

        // generate blocks
        bricksRemaining = levelGenerator.ChangeLevel(level);

        // update level UI display
        uiController.ShowLevel("Level " + level.ToString());

        // show level up text
        uiController.ShowLevelUpText("Level " + level.ToString());
    }

    public void RestartGame()
    {
        // Reload scene to restart game
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
        
        StartGame();
    }

}
