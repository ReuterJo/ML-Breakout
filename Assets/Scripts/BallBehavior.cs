using System.Collections;
using System.Collections.Generic;
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
    public float maxVelocity = 8f;
    
    [Tooltip("Sets the minimum ball velocity in the Y axis")]
    public float minVelocityY = 3.0f;
    
    [Tooltip("Sets the initial ball velocity")]
    public float startingVelocity = 3.5f;
    
    [Tooltip("Sets the game manager object")]
    public GameManager gameManager;
    
    [Tooltip("Sets the ball Component object")]
    private Rigidbody2D ball;
    
    [Tooltip("Sets the ball frozen state")]
    private bool frozen = true;


    void Start()
    // Loads the ball component and sets the ball starting position at the start of the game
    {
        ball = GetComponent<Rigidbody2D>();
        Reset();
    }

    void Update()
    // Regulates the ball position and velocity while in play
    {
        // Don't update the ball if it is frozen
        if (frozen)
        {
            ball.velocity = Vector2.zero;
            return;
        }    
        // Reset the ball position if it has fallen below the game Y axis
        if(transform.position.y < minY) 
        {
            gameManager.loseLife();
            Reset();
        }
        // Get the current velocity
        Vector2 currentVelocity = ball.velocity;

        // Check the direction of travel (up or down)
        if (currentVelocity.y > 0)
        {
            // Ensure minimum Y velocity if moving upwards
            if (currentVelocity.y < minVelocityY)
            {
                currentVelocity.y = minVelocityY;
                ball.velocity = currentVelocity;
            }
        }
        else if (currentVelocity.y < 0)
        {
            // Ensure minimum Y velocity if moving downwards
            if (currentVelocity.y > -minVelocityY)
            {
                currentVelocity.y = -minVelocityY;
                ball.velocity = currentVelocity;
            }
        }
        // Constrain total velocity
        if(ball.velocity.magnitude > maxVelocity)
        {
            ball.velocity = Vector2.ClampMagnitude(ball.velocity, maxVelocity);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    // Function used for destroying bricks when the ball collides with them
    {
        if(collision.gameObject.CompareTag("Brick"))
        {
            Destroy(collision.gameObject);
            gameManager.scoreBrick();
        }
    }

    /// <summary>
    /// Freeze the balls movement
    /// </summary>
    public void Freeze()
    {
        frozen = true;
    }

    /// <summary>
    /// Unfreezes the balls movement
    /// </summary>
    public void Unfreeze()
    {
        frozen = false;
    }

    public void Reset()
    // Randomly reset the ball position with position and velocity
    {
        float random = Random.Range(0f, 1f);  // stores randomly generated numbers  
        int side = Mathf.RoundToInt(random);  // stores a randomly generated 0 or 1
        if (side == 0)
        {
            random = Random.Range(-4f, -2f);
            transform.position = new Vector2(random, 0f);
            ball.velocity = new Vector2(1f, -1f) * startingVelocity;
        }
        else
        {
            random = Random.Range(2f, 4f);
            transform.position = new Vector2(random, 0f);
            ball.velocity = new Vector2(-1f, -1f) * startingVelocity;
        }
    }

    public void ChangeBallVelocity(float percent)
    {
        startingVelocity *= (1 + percent);
    }
}