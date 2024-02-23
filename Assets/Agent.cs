using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class ShootairAgent : Agent
{
    Rigidbody2D rBody;
    BehaviorParameters behaviorParameters;
    AgentSettings agentSettings;
    public GameObject bulletPrefab;
    public Transform firingPoint;

    EnvironmentController envController;

    EnvironmentParameters resetParams;
    
    void Start()
    {
        envController = FindObjectOfType<EnvironmentController>();
    }
    
    public override void Initialize()
    {
        base.Initialize();
        rBody = GetComponent<Rigidbody2D>();
        agentSettings = FindObjectOfType<AgentSettings>();
        behaviorParameters = gameObject.GetComponent<BehaviorParameters>();

        resetParams = Academy.Instance.EnvironmentParameters;
    }

    void Update()
    {
         Cursor.visible = false;
         Cursor.lockState = CursorLockMode.Locked;
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("target"))
        {
            envController.ResolveEvent(Event.collisionWithTarget);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agent position
        sensor.AddObservation(this.transform.localPosition);
        
        // Agent rotation
        sensor.AddObservation(this.transform.rotation);

        // Firing point position and rotation
        sensor.AddObservation(firingPoint.position);
        sensor.AddObservation(firingPoint.rotation);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.y);
    }

    public void MoveAgent(ActionBuffers actionBuffers)
    {
        float forwardAmount = 0f;       
        // Convert first action to moving forward or backwards
        if (actionBuffers.DiscreteActions[0] == 1f)
        {
            forwardAmount = 1f;
        }
        else if (actionBuffers.DiscreteActions[0] == 2f)
        {
            forwardAmount = -1f;
        }

        // Convert the second action to turning left or right
        float turnAmount = 0f;
        if (actionBuffers.DiscreteActions[1] == 1f)
        {
            turnAmount = -1f;
        }
        else if (actionBuffers.DiscreteActions[1] == 2f)
        {
            turnAmount = 1f;
        }
        if ((actionBuffers.ContinuousActions[1] == 1 && agentSettings.fireTimer <= 0f) || (agentSettings.autoShoot && agentSettings.fireTimer <= 0f))
        {
            Shoot();
            agentSettings.fireTimer = agentSettings.fireRate;
        } else
        {
            agentSettings.fireTimer -= Time.deltaTime;
        }

        // Apply movement and shoot
        rBody.MovePosition(transform.position + transform.up * forwardAmount * agentSettings.moveSpeed * Time.fixedDeltaTime + transform.right * turnAmount * agentSettings.moveSpeed * Time.fixedDeltaTime);
        transform.Rotate(transform.forward * -actionBuffers.ContinuousActions[0] * agentSettings.turnSpeed * Time.fixedDeltaTime);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        MoveAgent(actionBuffers);
    }

    private void Shoot()
    {
        Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int forwardAction = 0;
        int turnAction = 0;
        
        if (Input.GetKey(KeyCode.W))
        {
            // move forward
            forwardAction = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            // move forward
            forwardAction = 2;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            // turn left
            turnAction = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // turn right
            turnAction = 2;
        }

        // Put the actions into the array
        actionsOut.DiscreteActions.Array[0] = forwardAction;
        actionsOut.DiscreteActions.Array[1] = turnAction;
        
        actionsOut.ContinuousActions.Array[0] = Input.GetAxisRaw("Mouse X");
        actionsOut.ContinuousActions.Array[1] = Input.GetKey(KeyCode.Space) ? 1f : 0f;
    }
}