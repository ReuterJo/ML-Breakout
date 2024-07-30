using UnityEngine;
using TMPro;

public class BallBehavior : MonoBehaviour
{
    [Tooltip("Sets the game manager object")]
    public GameManager gameManager;
    [Tooltip("The VFX created when a brick is destroyed")]
    public GameObject onCollisionEffect;
    [Tooltip("The velocity text for the debug UI")]
    public TextMeshProUGUI velocityText;

    // Game dynamic variables
    private float minY = -5.5f;
    private float maxVelocity = 7.0f;
    private float minVelocityY = 3.5f;    
    private Rigidbody2D ball;
    private bool frozen = true;
    private AudioSource ballAudio;
    private Vector2 previousVelocity;

    /// <summary>
    /// Resets ball initial position and velocity
    /// </summary>
    public void Reset()
    // Randomly reset the ball position with position and velocity
    {
        this.ball = GetComponent<Rigidbody2D>();
        float random = UnityEngine.Random.Range(0f, 1f);  // stores randomly generated numbers  
        int side = Mathf.RoundToInt(random);  // stores a randomly generated 0 or 1
        Vector2 leftSide;
        Vector2 rightSide;
        ScreenPosition screenPosition = gameManager.GetScreenPosition();
        float vertExtent = Camera.main.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        // Centered Game
        if (screenPosition == ScreenPosition.Center)
        {
            leftSide = new Vector2(-horzExtent/2f + 1f, -1f);
            rightSide = new Vector2(1f, horzExtent/2f - 1f);
        }
        // Left Game
        else if (screenPosition == ScreenPosition.Left)
        {
            leftSide = new Vector2(-horzExtent + 1f, -horzExtent/2f - 1f);
            rightSide = new Vector2(-horzExtent/2f + 1f, -1f);
        }
        // Right Game
        else
        {
            leftSide = new Vector2(1f, horzExtent/2f - 1f);
            rightSide = new Vector2(horzExtent/2f + 1f, horzExtent - 1f);
        }
        if (side == 0)
        {
            random = UnityEngine.Random.Range(leftSide.x, leftSide.y);
            // Set the ball velocity to 1/2 the maxVelocity split in 2 axis
            this.ball.velocity = new Vector2(1f, -1f) * (this.maxVelocity / 2f);
        }
        else
        {
            random = UnityEngine.Random.Range(rightSide.x, rightSide.y);
            this.ball.velocity = new Vector2(-1f, -1f) * (this.maxVelocity / 2f);
        }
        // Start the ball position from the randomly generated settings above
        this.transform.position = new Vector2(random, 0f);
    }

    /// <summary>
    /// Increases the ball maximum and minimum velocities
    /// </summary>
    public void ChangeBallVelocity()
    {
        this.maxVelocity += 1;
        this.minVelocityY += 0.5f;
    }

    /// <summary>
    /// Returns the Y position of the ball
    /// </summary>
    /// <returns>Y position of ball</returns>
    public float GetBallYPosition()
    {
        return this.ball.transform.position.y;
    }

    /// <summary>
    /// Freeze the ball's movement
    /// </summary>
    public void Freeze()
    {
        this.previousVelocity = this.ball.velocity;
        this.ball.velocity = Vector2.zero;
        this.frozen = true;
    }

    /// <summary>
    /// Unfreezes the ball's movement
    /// </summary>
    public void Unfreeze()
    {
        this.ball.velocity = this.previousVelocity;
        this.frozen = false;
    }

    void Start()
    // Loads the ball component and sets the ball starting position at the start of the game
    {
        this.ball = GetComponent<Rigidbody2D>();
        this.ballAudio = GetComponent<AudioSource>();
        velocityText.text = "";
        if (gameManager.debug) velocityText.gameObject.SetActive(true);
        else velocityText.gameObject.SetActive(false);
    }

    void Update()
    // Regulates the ball position and velocity while in play
    {
        // Don't update the ball if it is frozen
        if (this.frozen) return;
        
        // Reset the ball position if it has fallen below the game Y axis
        if(transform.position.y < minY) 
        {
            this.gameManager.loseLife();
            Reset();
        }

        // NOTE: The ball will gain velocity when hitting the edge of an object (brick) or object with 
        // exiting velocity (moving paddle).  These functions enforce a set magnitude with a minimum Y 
        // axis velocity.

        Vector2 corrected;
        corrected = ball.velocity.normalized * maxVelocity;
        this.ball.velocity = corrected;

        // Clamp the ball y magnitude to a minimum value
        if (Mathf.Abs(this.ball.velocity.y) < this.minVelocityY) {
            float diff = this.minVelocityY - Mathf.Abs(this.ball.velocity.y);

            // If the ball is going upwards, shift the vector upwards
            if (this.ball.velocity.y >= 0)
            {
                corrected = new Vector2(this.ball.velocity.x - diff, this.ball.velocity.y + diff);
            }
            else
            {
                corrected = new Vector2(this.ball.velocity.x - diff, this.ball.velocity.y - diff);
            }
            this.ball.velocity = corrected;
        }

        if (gameManager.debug)
        {
            // FOR DEV - update velocity display
            float xVelocity = ball.velocity.x;
            float yVelocity = ball.velocity.y;
            velocityText.text = $"X: {xVelocity:F2}\nY: {yVelocity:F2}\nTotal: {ball.velocity.magnitude:F2}";
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    // Function used for destroying bricks when the ball collides with them
    {
        // Only play audio for the player game
        if(this.gameManager.playerType == PlayerType.Player || this.gameManager.playerType == PlayerType.Single) this.ballAudio.Play(0);
        if(collision.gameObject.CompareTag("Brick"))
        {
            Destroy(collision.gameObject);
            // VFX of brick break
            GameObject explosion = Instantiate(onCollisionEffect, collision.transform.position, collision.transform.rotation);
            Destroy(explosion, 1);
            this.gameManager.scoreBrick();
        }
    }

}