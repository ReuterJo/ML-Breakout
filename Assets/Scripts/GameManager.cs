using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.SceneManagement;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;

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

    private bool change_level = false;
    private bool configured = false;

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
    /// The current game state
    /// </summary>
    public GameState State { get; private set; } = GameState.Default;

    public void Configure(bool multi_level, 
                    bool training_mode, 
                    bool debug, 
                    PlayerType playerType, 
                    GameManager opponentGame,
                    string model_path
                    )
    {
        Debug.Log("Game Configure Called.");
        this.multi_level = multi_level;
        this.training_mode = training_mode;
        this.debug = debug;
        this.playerType = playerType;
        this.thisGame = this;
        this.opponentGame = opponentGame;
        
        // Determine screen position and set it
        switch (this.playerType)
        {
            case PlayerType.Agent:
                this.screenPosition = ScreenPosition.Right;
                break;
            case PlayerType.Player:
                this.screenPosition = ScreenPosition.Left;
                break;
            case PlayerType.Single:
                this.screenPosition = ScreenPosition.Center;
                break;
        }
        SetScreenPosition();

        Debug.Log("Game Configured.");
        // call to load correct agent model
        this.agentBehavior.Configure(model_path);
        this.configured = true;
    }

    void SetScreenPosition()
    {
        // Update Game Area GameObject position
        GameObject gameArea = GameObject.Find("GameArea");

        float vertExtent = Camera.main.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;
        
        Vector3 newPosition = gameArea.transform.position;

        // UI shift is hardcoded to match previous scene
        switch (this.screenPosition)
        {
            case ScreenPosition.Left:
                newPosition = new Vector3(-horzExtent / 2f, newPosition.y, newPosition.z);
                uiController.SetScreenPosition(0f);
                levelText.transform.position += new Vector3(0, 0, 0);
                ballBehavior.velocityText.transform.position += new Vector3(0, 0, 0);
                break;
            case ScreenPosition.Center:
                newPosition = new Vector3(0f, newPosition.y, newPosition.z);
                uiController.SetScreenPosition(482f);
                levelText.transform.position += new Vector3(482f, 0, 0);
                ballBehavior.velocityText.transform.position += new Vector3(482f, 0, 0);
                break;
            case ScreenPosition.Right:
                newPosition = new Vector3(horzExtent / 2f, newPosition.y, newPosition.z);
                uiController.SetScreenPosition(964f);
                levelText.transform.position += new Vector3(964f, 0, 0);
                ballBehavior.velocityText.transform.position += new Vector3(964f, 0, 0);
                break;
        }
        gameArea.transform.position = newPosition;
    }

    public ScreenPosition GetScreenPosition()
    {
        return this.screenPosition;
    }

    private IEnumerator WaitForConfiguration()
    {
        // Wait until the condition is met
        Debug.Log("Waiting to start.");
        yield return new WaitUntil(() => configured);
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    public async void StartGame()
    {
        if (!this.configured) StartCoroutine(this.WaitForConfiguration());
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

        // Begin countdown timer if not in training
        if (training_mode)
        {
        this.uiController.CountdownTimer(this.playerType);
        await Task.Delay(5000);
        }

        // Update lives and score
        this.uiController.ShowLives(this.lives.ToString() + " Lives");
        this.uiController.ShowScore("Score " + this.score.ToString());

        if (debug) this.levelText.gameObject.SetActive(true);
        else this.levelText.gameObject.SetActive(false);

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
        Time.timeScale = 0f;
        this.ballBehavior.Freeze();
        this.agentBehavior.Freeze();
    }

    private void ResumeGame()
    {
        State = GameState.Playing;
        Time.timeScale = 1f;
        this.ballBehavior.Unfreeze();
        this.agentBehavior.Unfreeze();
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
        // Check if the Escape key is pressed to pause/unpause game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (State == GameState.Playing)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }

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
                        // change game dynamics and points immediately
                        this.ChangeLevel();
                        if (this.change_level && ballBehavior.GetBallYPosition() < 0.0f)
                        {
                            // change bricks once ball is out of brick generator area
                            this.bricksRemaining = this.levelGenerator.ChangeLevel(this.level);
                            this.change_level = false;
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
        this.change_level = true;

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
