using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Runtime.CompilerServices;

public class AgentBehavior : Agent
{
    public float speed = 14.0f;             // sets the paddle speed
    public float maxX = 4.5f;                 // controls the max X dimension of the game
    [Tooltip("The game manager")]
    public GameManager gameManager;
    [Tooltip("The paddle ball")]
    public BallBehavior ballBehavior;
    private bool frozen = true;            // determines if the paddle is frozen or not

    /// <summary>
    /// Reset back to initial position
    /// </summary>
    public void Reset()
    {
        transform.position = new Vector2(0f, -4f);
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

    /// <summary>
    /// Give negative reward for losing a ball
    /// </summary>
    public void BallLost()
    {
        AddReward(-5f);
    }

    /// <summary>
    /// Give positive reward for breaking a brick
    /// </summary>
    public void BrickDestoryed()
    {
        AddReward(1f);
    }

    /// <summary>
    /// Give a very small reward for keeping the ball moving
    /// </summary>
    public void BallMoving()
    {
        AddReward(0.001f);
    }

    /// <summary>
    /// Ends the training episode
    /// </summary>
    public void EndTrainingEpisode()
    {
        EndEpisode();
    }

    /// <summary>
    /// Define what happens at the beginning of a training episode
    /// </summary>
    public override void OnEpisodeBegin()
    {
        gameManager.StartGame();
    }

    /// <summary>
    /// Define behavior based upon actions received
    /// </summary>
    /// <param name="actions">Actions provided by ML model</param>
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Don't update the paddle if it is frozen
        if (frozen) return;

        // get horizontal input from model
        float movementHorizontal = actions.ContinuousActions[0];

        // limit paddle to the boundaries of the screen window
        if ((movementHorizontal > 0 && transform.position.x < maxX) || (movementHorizontal < 0 && transform.position.x > -maxX)) {
            
            // vector of 1 in x and 0 in y, delta time ensures movement speed is time dependant across different devices
            transform.position += Vector3.right * movementHorizontal * speed * Time.deltaTime;
        }
    }

    /// <summary>
    /// Collect observations for ML model to process
    /// </summary>
    /// <param name="sensor">The vector sensor output</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        // Paddle observations for x and y position and velocity
        sensor.AddObservation(this.transform.position.x);
        sensor.AddObservation(this.transform.position.y);

        // Ball observations for x and y position and velocity
        sensor.AddObservation(ballBehavior.transform.position.x);
        sensor.AddObservation(ballBehavior.transform.position.y);
    }

    /// <summary>
    /// Define heuristic behavior (i.e. manual control)
    /// </summary>
    /// <param name="actionsOut"></param>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        ActionSegment<float> continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = horizontalInput;
    }

    /// <summary>
    /// Give a small reward for bouncing the ball
    /// </summary>
    /// <param name="collision">Collision event between GameObjects</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            AddReward(0.1f);
        }
    }
}
