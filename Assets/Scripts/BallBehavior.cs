using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class BallBehavior : MonoBehaviour
/**
 * @brief Controls the ball behavior in the game.
 *
 * @return void.
 */
{
    private float minY = -5.5f;
    private float maxVelocity = 8.0f;
    private float minVelocityY = 3.5f;
    
    [Tooltip("Sets the game manager object")]
    public GameManager gameManager;
    
    [Tooltip("The VFX created when a brick is destroyed")]
    public GameObject onCollisionEffect;

    public TextMeshProUGUI velocityText;
    
    private Rigidbody2D ball;
    private bool frozen = true;
    private AudioSource ballAudio;


    void Start()
    // Loads the ball component and sets the ball starting position at the start of the game
    {
        this.ball = GetComponent<Rigidbody2D>();
        this.ballAudio = GetComponent<AudioSource>();
        velocityText.text = "";
        this.Reset();
        if (gameManager.debug) velocityText.gameObject.SetActive(true);
        else velocityText.gameObject.SetActive(false);
    }

    void Update()
    // Regulates the ball position and velocity while in play
    {
        // Don't update the ball if it is frozen
        if (this.frozen)
        {
            this.ball.velocity = Vector2.zero;
            return;
        }    
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
        // depreciated - float factor = maxVelocity / ball.velocity.magnitude;
        // depreciated - corrected = new Vector2(this.ball.velocity.x * factor, this.ball.velocity.y * factor);
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
        if(this.gameManager.playerType == PlayerType.Player) this.ballAudio.Play(0);
        if(collision.gameObject.CompareTag("Brick"))
        {
            Destroy(collision.gameObject);
            // VFX of brick break
            GameObject explosion = Instantiate(onCollisionEffect, collision.transform.position, collision.transform.rotation);
            Destroy(explosion, 1);
            this.gameManager.scoreBrick();
        }
    }

    /// <summary>
    /// Freeze the balls movement
    /// </summary>
    public void Freeze()
    {
        this.frozen = true;
    }

    /// <summary>
    /// Unfreezes the balls movement
    /// </summary>
    public void Unfreeze()
    {
        this.frozen = false;
    }

    public void Reset()
    // Randomly reset the ball position with position and velocity
    {
        this.ball = GetComponent<Rigidbody2D>();
        float random = UnityEngine.Random.Range(0f, 1f);  // stores randomly generated numbers  
        int side = Mathf.RoundToInt(random);  // stores a randomly generated 0 or 1
        (float x, float y) leftSide = (0f, 0f);
        (float x, float y) rightSide = (0f, 0f);
        ScreenPosition screenPosition = gameManager.GetScreenPosition();
        // Centered Game
        if (screenPosition == ScreenPosition.Center)
        {
            leftSide = (-4f, -2f);
            rightSide = (2f, 4f);
        }
        // Left Game
        else if (screenPosition == ScreenPosition.Left)
        {
            leftSide = (-8f, -6f);
            rightSide = (-2f, 0f);
        }
        // Right Game
        else
        {
            leftSide = (0f, 2f);
            rightSide = (6f, 8f);
        }
        if (side == 0)
        {
            random = UnityEngine.Random.Range(leftSide.x, leftSide.y);

        }
        else
        {
            random = UnityEngine.Random.Range(rightSide.x, leftSide.y);
        }
        // Start the ball position from the randomly generated settings above
        this.transform.position = new Vector2(random, 0f);
        // Set the ball velocity to 1/2 the maxVelocity split in 2 axis
        this.ball.velocity = new Vector2(1f, -1f) * (this.maxVelocity / 2);
    }

    public void ChangeBallVelocity()
    {
        this.maxVelocity += 1;
        this.minVelocityY += 0.5f;
    }

    public float GetBallYPosition()
    {
        return this.ball.transform.position.y;
    }
}