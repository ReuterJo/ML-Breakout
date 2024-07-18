using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using System.Runtime.InteropServices;

public class AgentBehavior : Agent
{
    [Tooltip("Sets the paddle x axis speed")]
    public float speed = 10.0f;
    
    [Tooltip("Sets the allowable maximum X value in gameplay")]
    public float maxX = 4.5f;
    
    [Tooltip("Sets the GameManager object")]
    public GameManager gameManager;
    
    [Tooltip("Sets the BallBehavior script")]
    public BallBehavior ballBehavior;

    public float paddleWidth = 2.0f;
    
    [Tooltip("Sets the paddle frozen state")]
    private bool frozen = true;            // determines if the paddle is frozen or not

    private bool trainingMode;
    private PolygonCollider2D paddleCollider;
    private Rigidbody2D ballRd;
    private float colliderWidth;
    private float ballLostReward = -5f;
    private float ballMovingReward = 0.001f;
    private float ballHitReward = 0.1f;
    private float brickDestoryedReward = 1f;

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
        AddReward(ballLostReward);
    }

    /// <summary>
    /// Give positive reward for breaking a brick
    /// </summary>
    public void BrickDestoryed()
    {
        AddReward(brickDestoryedReward);
    }

    /// <summary>
    /// Give a very small reward for keeping the ball moving
    /// </summary>
    public void BallMoving()
    {
        AddReward(ballMovingReward);
    }

    /// <summary>
    /// Ends the training episode
    /// </summary>
    public void EndTrainingEpisode()
    {
        EndEpisode();
    }

    /// <summary>
    /// Initialize the agent
    /// </summary>
    public override void Initialize()
    {
        ballRd = ballBehavior.GetComponent<Rigidbody2D>();
        trainingMode = gameManager.training_mode;
        if(!trainingMode) MaxStep = 0;
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
        // Observations of ball x and y position
        sensor.AddObservation(ballBehavior.transform.position.x);
        sensor.AddObservation(ballBehavior.transform.position.y);

        // Observations of ball x and y velocity
        sensor.AddObservation(ballRd.velocity.x);
        sensor.AddObservation(ballRd.velocity.y);

        // Paddle observations for x position of left and right edge
        paddleCollider = this.GetComponent<PolygonCollider2D>();
        colliderWidth = paddleCollider.bounds.size.x;
        sensor.AddObservation(this.transform.position.x + colliderWidth / 2);
        sensor.AddObservation(this.transform.position.x - colliderWidth / 2);
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
            AddReward(ballHitReward);
        }
    }

    public void ChangePaddleScale(float change){
        transform.localScale = new Vector3(paddleWidth - change, 
                                transform.localScale.y, 
                                transform.localScale.z);
    }
}
