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
    [Tooltip("The bottom of the screen")]
    public float minY = -5.5f;              // sets the bottom of the screen
    [Tooltip("The maximum ball velocity")]
    public float maxVelocity = 8f;          // sets the maximum ball velocity
    [Tooltip("The minimum ball velocity")]
    public float minVelocityY = 3.0f;       // sets the minimum Y velocity
    [Tooltip("The starting ball velocity")]
    public float startingVelocity = 3.5f;   // sets the starting velocity
    [Tooltip("The game manager")]
    public GameManager gameManager;         // the game manager
    private Rigidbody2D rb;                 // stores the ball Component
    private bool frozen = false;            // determins if the ball is frozen or not


    void Start()
    // Loads the ball component and sets the ball starting position at the start of the game
    {
        rb = GetComponent<Rigidbody2D>();
        Reset();
    }

    void Update()
    // Regulates the ball position and velocity while in play
    {
        // Don't update the ball if it is frozen
        if (frozen)
        {
            rb.velocity = Vector2.zero;
            return;
        }    
        // Reset the ball position if it has fallen below the game Y axis
        if(transform.position.y < minY) 
        {
            gameManager.loseLife();
            Reset();
        }
        // Get the current velocity
        Vector2 currentVelocity = rb.velocity;

        // Check the direction of travel (up or down)
        if (currentVelocity.y > 0)
        {
            // Ensure minimum Y velocity if moving upwards
            if (currentVelocity.y < minVelocityY)
            {
                currentVelocity.y = minVelocityY;
                rb.velocity = currentVelocity;
            }
        }
        else if (currentVelocity.y < 0)
        {
            // Ensure minimum Y velocity if moving downwards
            if (currentVelocity.y > -minVelocityY)
            {
                currentVelocity.y = -minVelocityY;
                rb.velocity = currentVelocity;
            }
        }
        // Constrain total velocity
        if(rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
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
            rb.velocity = new Vector2(1f, -1f) * startingVelocity;
        }
        else
        {
            random = Random.Range(2f, 4f);
            transform.position = new Vector2(random, 0f);
            rb.velocity = new Vector2(-1f, -1f) * startingVelocity;
        }
    }
}