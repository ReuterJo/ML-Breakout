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
    private bool levelUpTextActive = false;
    private float levelUpTextStartTime;

    private bool generateBricks = false;

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
    public void StartGame()
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
                this.ShowLevelUpText("Level " + level.ToString());
            }
            else
            {
                // decrement level by one to generate correct level
                this.level -= 1;
                this.ChangeLevel();
                this.bricksRemaining = this.levelGenerator.ChangeLevel(this.level);
            }
            
            this.uiController.ShowLevel("Level " + level.ToString());
        }

        // Update lives and score
        this.uiController.ShowLives(this.lives.ToString() + " Lives");
        this.uiController.ShowScore("Score " + this.score.ToString());

        if (debug) this.levelText.gameObject.SetActive(true);
        else this.levelText.gameObject.SetActive(true);

        // Begin the level timer
        this.levelStartTime = Time.time;

/*         // Begin countdown timer if not in training
         if (!training_mode)
        {
            int counter = 5;
            string player = "Player Game\nStarting In:\n";
            if (playerType == PlayerType.Agent) player = "Agent Game \nStarting In:\n";
            while (counter != 0)
            {
                this.uiController.ShowLevelUpText(player + counter);
                await Task.Delay(1000);
                counter--;
            }
            this.uiController.HideLevelUpText();
        } */


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
        if (this.playerType == PlayerType.Player) this.uiController.ShowPauseCanvas();
    }

    public void ResumeGame()
    {
        State = GameState.Playing;
        Time.timeScale = 1f;
        this.ballBehavior.Unfreeze();
        this.agentBehavior.Unfreeze();
        if (this.playerType == PlayerType.Player) this.uiController.HidePauseCanvas();
    }

    /// <summary>
    /// Ends the game
    /// </summary>
    private void EndGame()
    {

        if (this.playerType == PlayerType.Player)
        {
            // Pause Player Game
            this.ballBehavior.Freeze();
            this.agentBehavior.Freeze();
            State = GameState.Paused;

            // Player completed game - apply ball bonus
            if (level == 5 && bricksRemaining == 0)
            {
                int ballBonus = this.lives - 1 * 1000;
                this.score += ballBonus;
                this.ShowLevelUpText("Game Ended\nBall Bonus: " + ballBonus.ToString());
            }
            // Two Player Winner Display
            if (opponentGame != null)
            {
                if (opponentGame.GetScore() > this.score) this.ShowLevelUpText("Agent Wins!");
                else this.ShowLevelUpText("Player Wins!");
            }
            // Single Player Game Over Display
            else
            {
                this.ShowLevelUpText("Game Over!");
            }

            State = GameState.Gameover;
            Time.timeScale = 0f;
            
            // Check if the leaderboard needs to be updated
            this.leaderboardManager.AddScore(this.score);
        }
        else
        {
            this.ballBehavior.Freeze();
            this.agentBehavior.Freeze();
            // Agent completed game - apply ball bonus
            if (level == 5 && bricksRemaining == 0)
            {
                int ballBonus = this.lives - 1 * 1000;
                this.score += ballBonus;
                this.ShowLevelUpText("Game Ended\nBall Bonus: " + ballBonus.ToString());
            }
            if (opponentGame != null)
            {
                if (opponentGame.GetScore() > this.score) this.ShowLevelUpText("Agent Wins!");
                else this.ShowLevelUpText("Player Wins!");
            }
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

        // Hide level up text if shown
        if (this.levelUpTextActive && Time.time - levelUpTextStartTime >= 2f) this.HideLevelUpText();

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
                // check for move to next level
                else
                {
                    if (this.bricksRemaining == 0 || (this.level == 1 && this.bricksRemaining < 27))
                    {
                        // change game dynamics and points immediately
                        if (!this.generateBricks)
                        {
                            this.ChangeLevel();
                            this.generateBricks = true;
                        }
                    }
                    // change bricks once ball is out of brick generator area
                    if (this.generateBricks && ballBehavior.GetBallYPosition() < 0.0f)
                    {
                        this.bricksRemaining = this.levelGenerator.ChangeLevel(this.level);
                        this.generateBricks = false;
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

        // update level UI display
        this.uiController.ShowLevel("Level " + this.level.ToString());

        // show level up text
        this.ShowLevelUpText("Starting Level " + this.level.ToString() + "\nBonus: " + bonus);

        Debug.Log("Changed Level");
    }

    private void ShowLevelUpText(string text)
    {
        this.uiController.ShowLevelUpText(text);
        this.levelUpTextActive = true;
        this.levelUpTextStartTime = Time.time;
    }

    private void HideLevelUpText()
    {
        this.uiController.HideLevelUpText();
        this.levelUpTextActive = false;
    }

    public void RestartGame()
    {
        // Unfreeze objects before restart
        this.ResumeGame();
        // Reload scene to restart game
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
        this.StartGame();
    }

    public int GetScore()
    {
        return this.score;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
