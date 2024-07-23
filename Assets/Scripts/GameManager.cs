using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.SceneManagement;
using TMPro;
using System.Threading.Tasks;

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

    public bool debug = false;

    public PlayerType playerType;

    public TextMeshProUGUI levelText;

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
    private float levelPaddleSizeSubtractor = 0.15f;
    private float levelBonusMultiplier = 1.5f;

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

        this.thisGame.transform.position = newPosition;
    }

    public ScreenPosition GetScreenPosition()
    {
        return this.screenPosition;
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    public async void StartGame()
    {
        // Set the game state to preparing
        State = GameState.Preparing;
        this.level = starting_level;
        this.lives = 5;
        this.score = 0;
        this.levelText.text = "";
        this.leaderboardManager.HideLeaderboard();

        // Generate level

        if (!multi_level)
        {
            this.uiController.HideLevel();
            this.bricksRemaining = levelGenerator.ChangeLevel(level);
        }
        else
        {
            if (this.level == 1)
            {
                this.bricksRemaining = levelGenerator.ChangeLevel(level);
                this.uiController.ShowLevelUpText("Level " + level.ToString());
            }
            else
            {
                // decrement level by one to generate correct level
                this.level -= 1;
                this.ChangeLevel();
            }
            
            this.uiController.ShowLevel("Level " + level.ToString());
        }

        // Begin countdown timer
        this.uiController.CountdownTimer(this.playerType);
        await Task.Delay(5000);

        // Update lives and score
        this.uiController.ShowLives(this.lives.ToString() + " Lives");
        this.uiController.ShowScore("Score " + this.score.ToString());

        // Begin the level timer
        this.levelStartTime = Time.time;

        // Reset paddle ball
        this.ballBehavior.Reset();
        this.agentBehavior.Reset();

        // Unfreeze player and ball
        this.ballBehavior.Unfreeze();
        this.agentBehavior.Unfreeze();

        // Set the game state to playing
        State = GameState.Playing;
    }

    private void Awake()
    {
        this.ballRb = ballBehavior.GetComponent<Rigidbody2D>();
    }

        /// <summary>
    /// Called when a brick is destroyed
    /// </summary>
    public void scoreBrick()
    {   
        this.score += this.brickValue;
        this.uiController.ShowScore("Score " + this.score.ToString());
        this.agentBehavior.BrickDestoryed();
    }

    /// <summary>
    /// Called when a life is lost
    /// </summary>
    public void loseLife()
    {
        this.lives--;
        this.uiController.ShowLives(this.lives.ToString() + " Lives");
        this.agentBehavior.BallLost();
    }

    private void PauseGame()
    {
        State = GameState.Paused;
        this.ballBehavior.Freeze();
        this.agentBehavior.Freeze();
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
        this.ballBehavior.Freeze();
        this.agentBehavior.Freeze();

        if (playerType == PlayerType.Agent)
        {
            this.uiController.ShowLevelUpText("Game Ended");
        }
        else
        {
            // Check if the leaderboard needs to be updated
            this.leaderboardManager.AddScore(this.score);
        }


    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        if (State == GameState.Playing)
        {
            // Reward agent for ball moving
            if (this.ballRb.velocity.magnitude > 0) this.agentBehavior.BallMoving();

            // Check to see if the player has lost all lives
            if (this.lives == 0)
            {
                if (!this.training_mode) EndGame();
                else this.agentBehavior.EndTrainingEpisode();   
            }
            this.bricksRemaining = levelGenerator.getBrickCount();

            // Single level game
            if (!this.multi_level)
            {
                // finished single-level game
                if (this.bricksRemaining == 0)
                {
                    if (!this.training_mode) EndGame();
                    else this.agentBehavior.EndTrainingEpisode();  
                }
            }
            // Multi-level game
            else
            {
                // finished multi-level game
                if (this.bricksRemaining == 0 && this.level == 5)
                {
                    if (!this.training_mode) EndGame();
                    else this.agentBehavior.EndTrainingEpisode();  
                }
                // move to the next level
                else
                {
                    if (this.bricksRemaining == 0 || (this.level == 1 && this.bricksRemaining < 27))
                    {
                        if (ballBehavior.GetBallYPosition() < 0.0f)
                        {
                            this.ChangeLevel();
                        }

                    }
                }
            }
        }
        if (debug)
        {
            float seconds = Time.time - this.levelStartTime;
            this.levelText.text = $"Bricks: {this.bricksRemaining}\nTime: {seconds:F2}\nLevel: {this.level}";
        }
    }

    public void ChangeLevel()
    {
        // increment level
        this.level += 1;

        // change brickValue 
        this.brickValue = (int) (this.brickValue * this.level * this.levelPointMultiplier);

        // give level bonus points based on time to complete level
        float elapsed = Time.time - levelStartTime;
        int bonus = (int) ((level * levelBonusMultiplier * 5000) / elapsed);
        this.score += bonus;
        this.levelStartTime = Time.time;

        // change paddle size
        this.agentBehavior.ChangePaddleScale(this.level * this.levelPaddleSizeSubtractor);

        // change ball velocity
        this.ballBehavior.ChangeBallVelocity();

        // generate blocks
        this.bricksRemaining = this.levelGenerator.ChangeLevel(this.level);

        // update level UI display
        this.uiController.ShowLevel("Level " + this.level.ToString());

        // show level up text
        StartCoroutine(this.uiController.ShowLevelUpText("Starting Level " + this.level.ToString() + "\nBonus: " + bonus));
    }

    public void RestartGame()
    {
        // Reload scene to restart game
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
        
        this.StartGame();
    }

}
