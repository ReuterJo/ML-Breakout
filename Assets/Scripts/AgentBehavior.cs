using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using System.Runtime.InteropServices;
using Unity.MLAgents.Policies;
using Unity.Barracuda;
using System.Threading.Tasks;

public class AgentBehavior : Agent
{
    private float speed = 10.0f;
    
    [Tooltip("Sets the GameManager object")]
    public GameManager gameManager;
    
    [Tooltip("Sets the BallBehavior script")]
    public BallBehavior ballBehavior;
    private ScreenPosition screenPosition;

    private BehaviorParameters behaviorParameters;

    private float minX;
    private float maxX;

    private float paddleWidth = 2.0f;
    
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

    public void Start()
    {   
        // Not used        
    }

    void Update()
    {
        RequestDecision();
    }

    public void SetScreenPosition()
    {
        this.screenPosition = this.gameManager.screenPosition;        
        float vertExtent = Camera.main.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;
        Debug.Log("The screen position is " + this.screenPosition.ToString());
        // Left game
        if (this.screenPosition == ScreenPosition.Left)
        {
            this.minX = -horzExtent;
            this.maxX = 0f;
            //this.transform.position = new Vector2(-horzExtent / 2f, -4f);
        }
        // Right game
        else if (this.screenPosition == ScreenPosition.Right)
        {
            this.minX = 0f;
            this.maxX = horzExtent;
            //this.transform.position = new Vector2(horzExtent / 2f, -4f);
        }
        // Centered game
        else
        {
            this.minX = -horzExtent / 2f;
            this.maxX = horzExtent / 2f;
            //this.transform.position = new Vector2(0f, -4f);
        }
    }

    public void Configure(string model_path)
    {
        this.behaviorParameters = GetComponent<BehaviorParameters>();
        if(this.gameManager.playerType == PlayerType.Agent)
        {
            behaviorParameters.BehaviorType = BehaviorType.Default;
            NNModel model = Resources.Load<NNModel>(model_path);
            this.SetModel("AgentBehavior", model);
        }
        else
        {
            behaviorParameters.BehaviorType = BehaviorType.HeuristicOnly;
        }
    }

    /// <summary>
    /// Reset back to initial position
    /// </summary>
    public void Reset()
    {

        this.screenPosition = gameManager.GetScreenPosition();
        // Left game
        if (this.screenPosition == ScreenPosition.Left)
        {
            transform.position = new Vector2(-4f, -4f);
        }
        // Right game
        else if (this.screenPosition == ScreenPosition.Right)
        {
            transform.position = new Vector2(4f, -4f);
        }
        // Centered game
        else
        {
            transform.position = new Vector2(0f, -4f);
        }
    }

    /// <summary>
    /// Freezes the paddle movement
    /// </summary>
    public void Freeze()
    {
        this.frozen = true;
    }

    /// <summary>
    /// Unfreezes the paddle movement
    /// </summary>
    public void Unfreeze()
    {
        this.frozen = false;
    }

    /// <summary>
    /// Give negative reward for losing a ball
    /// </summary>
    public void BallLost()
    {
        this.AddReward(this.ballLostReward);
    }

    /// <summary>
    /// Give positive reward for breaking a brick
    /// </summary>
    public void BrickDestoryed()
    {
        this.AddReward(this.brickDestoryedReward);
    }

    /// <summary>
    /// Give a very small reward for keeping the ball moving
    /// </summary>
    public void BallMoving()
    {
        this.AddReward(this.ballMovingReward);
    }

    /// <summary>
    /// Ends the training episode
    /// </summary>
    public void EndTrainingEpisode()
    {
        this.EndEpisode();
    }

    /// <summary>
    /// Initialize the agent
    /// </summary>
    public override void Initialize()
    {
        this.ballRd = this.ballBehavior.GetComponent<Rigidbody2D>();
        this.trainingMode = gameManager.training_mode;
        if(!this.trainingMode) MaxStep = 0;
    }

    /// <summary>
    /// Define what happens at the beginning of a training episode
    /// </summary>
    public override async void OnEpisodeBegin()
    {
        Debug.Log("The Agent has called StartGame");
        await Task.Delay(100);
        this.gameManager.StartGame();
    }

    /// <summary>
    /// Define behavior based upon actions received
    /// </summary>
    /// <param name="actions">Actions provided by ML model</param>
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Don't update the paddle if it is frozen
        if (this.frozen) return;

        // get horizontal input from model
        float movementHorizontal = actions.ContinuousActions[0];

        // limit paddle to the boundaries of the screen window
        if ((movementHorizontal > 0 && transform.position.x < maxX) || (movementHorizontal < 0 && transform.position.x > minX)) {
            
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
