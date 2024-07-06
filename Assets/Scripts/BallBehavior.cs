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
    public float minY = -5.5f;              // sets the bottom of the screen
    public float maxVelocity = 8f;          // sets the maximum ball velocity
    public float minVelocityY = 3.0f;       // sets the minimum Y velocity
    public float startingVelocity = 3.5f;   // sets the starting velocity
    int side;                               // stores a randomly generated 0 or 1
    float random;                           // stores randomly generated numbers
    Rigidbody2D rb;                         // stores the ball Component


    void Start()
    // Loads the ball component and sets the ball starting position at the start of the game
    {
        rb = GetComponent<Rigidbody2D>();
        Reset();
    }

    void Update()
    // Regulates the ball position and velocity while in play
    {
        // Reset the ball position if it has fallen below the game Y axis
        if(transform.position.y < minY) 
        {
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
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    // Function used for destroying bricks when the ball collides with them
    {
        if(collision.gameObject.CompareTag("Brick"))
        {
            Destroy(collision.gameObject);
        }
    }

    private void Reset()
    // Randomly reset the ball position with position and velocity
    {
        random = Random.Range(0f, 1f);
        side = Mathf.RoundToInt(random);
        if (side == 0)
        {
            random = Random.Range(-4f, -2f);
            transform.position = new Vector2(random, 0f);
            rb.velocity = new Vector2(3.5f, -3.5f);
        }
        else
        {
            random = Random.Range(2f, 8f);
            transform.position = new Vector2(random, 0f);
            rb.velocity = new Vector2(-3.5f, -3.5f);
        }
    }
}