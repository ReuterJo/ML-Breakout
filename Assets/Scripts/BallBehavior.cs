using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class BallBehavior : MonoBehaviour
/**
 * @brief Controls the ball behavior in the game.
 *
 * @return void.
 */
{
    [Tooltip("Sets the bottom of the screen to determine a lost ball")]
    public float minY = -5.5f;
    
    [Tooltip("Sets maximum total ball velocity")]
    public float maxVelocity = 7f;
    
    [Tooltip("Sets the minimum ball velocity in the Y axis")]
    public float minVelocityY = 3.0f;
    
    [Tooltip("Sets the game manager object")]
    public GameManager gameManager;
    
    [Tooltip("The VFX created when a brick is destroyed")]
    public GameObject onCollisionEffect;
    
    private Rigidbody2D ball;
    private bool frozen = true;
    private AudioSource ballAudio;


    void Start()
    // Loads the ball component and sets the ball starting position at the start of the game
    {
        this.ball = GetComponent<Rigidbody2D>();
        this.ballAudio = GetComponent<AudioSource>();
        this.Reset();
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
        // Get the current velocity
        Vector2 currentVelocity = ball.velocity;

        // Check the direction of travel (up or down)
        if (currentVelocity.y > 0)
        {
            // Ensure minimum Y velocity if moving upwards
            if (currentVelocity.y < this.minVelocityY)
            {
                currentVelocity.y = this.minVelocityY;
                this.ball.velocity = currentVelocity;
            }
        }
        else if (currentVelocity.y < 0)
        {
            // Ensure minimum Y velocity if moving downwards
            if (currentVelocity.y > -this.minVelocityY)
            {
                currentVelocity.y = -minVelocityY;
                this.ball.velocity = currentVelocity;
            }
        }
        // Constrain total velocity
        if(this.ball.velocity.magnitude > this.maxVelocity)
        {
            this.ball.velocity = Vector2.ClampMagnitude(this.ball.velocity, this.maxVelocity);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    // Function used for destroying bricks when the ball collides with them
    {
        this.ballAudio.Play(0);
        if(collision.gameObject.CompareTag("Brick"))
        {
            Destroy(collision.gameObject);
            Instantiate(onCollisionEffect, collision.transform.position, collision.transform.rotation);
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

    public void ChangeBallVelocity(float percent)
    {
        this.maxVelocity *= (1 + percent);
    }
}