using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.SceneManagement;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;

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

    public GameGenerator gameGenerator;

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

    private AudioSource levelUpAudio;
    private AudioSource lifeLostAudio;

    /// <summary>
    /// The current game state
    /// </summary>
    public GameState State { get; private set; } = GameState.Default;

    public void Configure(bool multi_level, 
                    bool training_mode, 
                    bool debug, 
                    PlayerType playerType, 
                    GameManager opponentGame,
                    string model_path,
                    GameGenerator gameGenerator
                    )
    {
        this.multi_level = multi_level;
        this.training_mode = training_mode;
        this.debug = debug;
        this.playerType = playerType;
        this.thisGame = this;
        this.opponentGame = opponentGame;
        this.gameGenerator = gameGenerator;

        // call to load correct agent model
        this.agentBehavior.Configure(model_path);

        // Configure audio
        this.levelUpAudio = GameObject.Find("LevelUpSFX").GetComponent<AudioSource>();
        this.lifeLostAudio = GameObject.Find("LifeLostSFX").GetComponent<AudioSource>();

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
        this.SetScreenPosition();
        this.agentBehavior.SetScreenPosition();
        
        // set color of agent to be different than player
        if (this.playerType == PlayerType.Agent)
        {
            SpriteRenderer agentSpriteRender = this.agentBehavior.GetComponent<SpriteRenderer>();
            agentSpriteRender.color = new Color(243f/255f, 83f/255f, 132f/255f, 255f/255f);
        }
    }

    void SetScreenPosition()
    {
        // Update Game Area GameObject position
        // using the incorrect logic here


        Transform gameAreaTransform = this.transform.Find("GameArea");

        float vertExtent = Camera.main.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;
        
        Vector3 newPosition = gameAreaTransform.position;

        // UI shift is hardcoded to match previous scene
        switch (this.screenPosition)
        {
            case ScreenPosition.Left:
                newPosition = new Vector3(-horzExtent / 2f, newPosition.y, newPosition.z);
                this.uiController.SetScreenPosition(0f);
                this.levelText.transform.position += new Vector3(0, 0, 0);
                this.ballBehavior.velocityText.transform.position += new Vector3(0, 0, 0);
                break;
            case ScreenPosition.Center:
                newPosition = new Vector3(0f, newPosition.y, newPosition.z);
                this.uiController.SetScreenPosition(482f);
                this.levelText.transform.position += new Vector3(482f, 0, 0);
                this.ballBehavior.velocityText.transform.position += new Vector3(482f, 0, 0);
                break;
            case ScreenPosition.Right:
                newPosition = new Vector3(horzExtent / 2f, newPosition.y, newPosition.z);
                this.uiController.SetScreenPosition(964f);
                this.levelText.transform.position += new Vector3(964f, 0, 0);
                this.ballBehavior.velocityText.transform.position += new Vector3(964f, 0, 0);
                break;
        }
        gameAreaTransform.position = newPosition;
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

        // Reset paddle ball
        this.ballBehavior.Reset();
        this.agentBehavior.Reset();

        // Freeze all game assets
        ballBehavior.Freeze();
        agentBehavior.Freeze();

        this.level = starting_level;
        this.lives = 5;
        this.score = 0;
        this.levelText.text = "";

        // Update lives and score
        this.uiController.ShowLives(this.lives.ToString() + " Lives");
        this.uiController.ShowScore("Score " + this.score.ToString());

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
        if (!training_mode)
        {
            this.uiController.CountdownTimer(this.playerType);
            await Task.Delay(5000);
        }

        if (debug) this.levelText.gameObject.SetActive(true);
        else this.levelText.gameObject.SetActive(false);

        // Begin the level timer
        this.levelStartTime = Time.time;

        // Unfreeze player and ball
        this.ballBehavior.Unfreeze();
        this.agentBehavior.Unfreeze();

        // Set the game state to playing
        State = GameState.Playing;
        Time.timeScale = 1f;
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
        if(this.playerType == PlayerType.Player || this.playerType == PlayerType.Single) this.lifeLostAudio.Play(0);
    }

    private void PauseGame()
    {
        State = GameState.Paused;
        Time.timeScale = 0f;
        this.ballBehavior.Freeze();
        this.agentBehavior.Freeze();
        this.uiController.ShowPauseCanvas();
    }

    public void ResumeGame()
    {
        this.uiController.HidePauseCanvas();
        State = GameState.Playing;
        this.ballBehavior.Unfreeze();
        this.agentBehavior.Unfreeze();
        if (opponentGame != null)
        {
            opponentGame.ballBehavior.Unfreeze();
            opponentGame.agentBehavior.Unfreeze();
            opponentGame.uiController.HidePauseCanvas();
        }
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Ends the game
    /// </summary>
    private async void EndGame()
    {
        // Set the game state to game over
        State = GameState.Gameover;

        // TODO: WILL NEED TO FIX THESE ONCE BRICKS ARE FIXED - USED FOR BEHAVIOR TESTING
        if (this.playerType == PlayerType.Agent)
        {
            Debug.Log("Player End Game");
            State = GameState.Paused;
            this.ballBehavior.Freeze();
            this.agentBehavior.Freeze();
            int ballBonus = this.lives - 1 * 1000;
            this.score += ballBonus;
            this.uiController.ShowLevelUpText("Game Ended\nBall Bonus: " + ballBonus.ToString());
            await Task.Delay(2000);
            if (opponentGame != null)
            {
                if (opponentGame.GetScore() > this.score) this.uiController.ShowLevelUpText("Agent Wins!");
                else this.uiController.ShowLevelUpText("Player Wins!");
            }
            await Task.Delay(2000);
            // Check if the leaderboard needs to be updated
            this.gameGenerator.AddToLeaderboard(this.score);
        }
        else
        {
            Debug.Log("Agent End Game");
            State = GameState.Paused;
            this.ballBehavior.Freeze();
            this.agentBehavior.Freeze();
            int ballBonus = this.lives - 1 * 1000;
            this.score += ballBonus;
            this.uiController.ShowLevelUpText("Game Ended\nBall Bonus: " + ballBonus.ToString());
            await Task.Delay(2000);
            if (opponentGame != null)
            {
                if (opponentGame.GetScore() > this.score) this.uiController.ShowLevelUpText("Agent Wins!");
                else this.uiController.ShowLevelUpText("Player Wins!");
            }
            await Task.Delay(2000);
            // Check if the leaderboard needs to be updated
            this.gameGenerator.AddToLeaderboard(this.score);
        }


    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        // Do nothing if the game is preparing
        if (State == GameState.Preparing) return;

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
                        if (ballBehavior.GetBallYPosition() < 0.0f)
                        {
                            // change bricks once ball is out of brick generator area
                            this.ChangeLevel();
                            this.bricksRemaining = this.levelGenerator.ChangeLevel(this.level);
                            // change game dynamics and points immediately
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

        // Play level up sound
        if(this.playerType == PlayerType.Player || this.playerType == PlayerType.Single)
            if(this.level > starting_level) this.levelUpAudio.Play(0);

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

        // update level UI display
        this.uiController.ShowLevel("Level " + this.level.ToString());

        // show level up text
        StartCoroutine(this.uiController.ShowLevelUpText("Starting Level " + this.level.ToString() + "\nBonus: " + bonus));
    }

    public void RestartGame()
    {
        this.uiController.HidePauseCanvas();
        this.gameGenerator.Restart();
    }

    public int GetScore()
    {
        return this.score;
    }

    public void MainMenu()
    {
        this.gameGenerator.ReturnToMenu();
    }

}
