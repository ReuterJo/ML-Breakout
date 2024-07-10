using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
/**
 * @brief Controls the player movement of the paddle in the X axis in the game.
 *
 * @return void.
 */
{
    public float speed = 14.0f;             // sets the paddle speed
    public float maxX = 6f;                 // controls the max X dimension of the game
    private float movementHorizontal;       // stores the user/agent movement input
    private Rigidbody2D rb;                 // sets the paddle Component
    private bool frozen = false;            // determines if the paddle is frozen or not

    // Start is called before the first frame update
    void Start()
    // Sets the paddle Component at the start of the game
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    // Regulates the paddle movement to the speed and dimensions of the game
    {
        // Don't update the paddle if it is frozen
        if (frozen) return;
        // get horizontal input from user
        movementHorizontal = Input.GetAxis("Horizontal");

        // limit paddle to the boundaries of the screen window
        if ((movementHorizontal > 0 && transform.position.x < maxX) || (movementHorizontal < 0 && transform.position.x > -maxX)) {
            
            // vector of 1 in x and 0 in y, delta time ensures movement speed is time dependant across different devices
            transform.position += Vector3.right * movementHorizontal * speed * Time.deltaTime;
        }
    }

    /// <summary>
    /// Freezes the paddle movement
    /// </summary>
    public void Freeze()
    {
        frozen = true;
    }

    /// <summary>
    /// Unfreezes the paddle movement
    /// </summary>
    public void Unfreeze()
    {
        frozen = false;
    }
}
