using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine.AI;

public class ShootairAgent : Agent
{
    Rigidbody2D rBody;
    BehaviorParameters behaviorParameters;
    AgentSettings agentSettings;
    public GameObject bulletPrefab;
    public Transform firingPoint;

    EnvironmentController envController;

    EnvironmentParameters resetParams;

    private float targetRotation = 0f;
    
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
        float turnAmount = 0f;
        
        if (actionBuffers.DiscreteActions[0] == 1f)
        {            
            if (actionBuffers.ContinuousActions[0] == 1 && (transform.eulerAngles.z == 0 || transform.eulerAngles.z == 180))
            {
                forwardAmount = transform.eulerAngles.z == 0 ? 1f : -1f;
            }
            else if (actionBuffers.ContinuousActions[0] == 1 && (transform.eulerAngles.z == 90 || transform.eulerAngles.z == 270))
            {
                turnAmount = transform.eulerAngles.z == 90 ? 1f : -1f;
            }
            else if (actionBuffers.ContinuousActions[0] != 1)
            {
                forwardAmount = 1f;
                targetRotation = 0f;
            }
        }
        else if (actionBuffers.DiscreteActions[0] == 2f)
        {            
            if (actionBuffers.ContinuousActions[0] == 1 && (transform.eulerAngles.z == 0 || transform.eulerAngles.z == 180))
            {
                forwardAmount = transform.eulerAngles.z == 0 ? -1f : 1f;
            }
            else if (actionBuffers.ContinuousActions[0] == 1 && (transform.eulerAngles.z == 90 || transform.eulerAngles.z == 270))
            {
                turnAmount = transform.eulerAngles.z == 90 ? -1f : 1f;
            }
            else if (actionBuffers.ContinuousActions[0] != 1)
            {
                forwardAmount = 1f;
                targetRotation = 180f;
            }
        }

        if (transform.eulerAngles.z != targetRotation && actionBuffers.ContinuousActions[0] != 1)
        {
            transform.rotation = Quaternion.Euler(0, 0, targetRotation);
        }

        if (actionBuffers.DiscreteActions[1] == 1f)
        {
            if (actionBuffers.ContinuousActions[0] == 1 && (transform.eulerAngles.z == 0 || transform.eulerAngles.z == 180))
            {
                turnAmount = transform.eulerAngles.z == 0 ? -1f : 1f;
            }
            else if (actionBuffers.ContinuousActions[0] == 1 && (transform.eulerAngles.z == 90 || transform.eulerAngles.z == 270))
            {
                forwardAmount = transform.eulerAngles.z == 90 ? 1f : -1f;
            }
            else if (actionBuffers.ContinuousActions[0] != 1)
            {
                forwardAmount = 1f;
                targetRotation = 90f;
            }
        }
        else if (actionBuffers.DiscreteActions[1] == 2f)
        {   
            if (actionBuffers.ContinuousActions[0] == 1 && (transform.eulerAngles.z == 0 || transform.eulerAngles.z == 180))
            {
                turnAmount = transform.eulerAngles.z == 0 ? 1f : -1f;
            }
            else if (actionBuffers.ContinuousActions[0] == 1 && (transform.eulerAngles.z == 90 || transform.eulerAngles.z == 270))
            {
                forwardAmount = transform.eulerAngles.z == 90 ? -1f : 1f;
            }
            else if (actionBuffers.ContinuousActions[0] != 1)
            {
                forwardAmount = 1f;
                targetRotation = 270f;
            }
        }

        if (transform.eulerAngles.z != targetRotation && actionBuffers.ContinuousActions[0] != 1)
        {
            transform.rotation = Quaternion.Euler(0, 0, targetRotation);
        }

        // turnAmount = 0f;

        // Apply movement
        // Debug.Log(transform.eulerAngles.z);
        Vector3 movementForward = transform.up * forwardAmount * agentSettings.moveSpeed * Time.fixedDeltaTime;
        Vector3 movementTurn = transform.right * turnAmount * agentSettings.moveSpeed * Time.fixedDeltaTime;
        Vector3 movement = movementForward + movementTurn;
        // Vector3 rotation = transform.eulerAngles + new Vector3(0, 0, turnAmount);
        rBody.MovePosition(transform.position + movement);
        // transform.rotation = Quaternion.Euler(rotation);
        
        if ((actionBuffers.ContinuousActions[0] == 1 && agentSettings.fireTimer <= 0f) || (agentSettings.autoShoot && agentSettings.fireTimer <= 0f))
        {
            Shoot();
            agentSettings.fireTimer = agentSettings.fireRate;
        } 
        else
        {
            agentSettings.fireTimer -= Time.deltaTime;
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        MoveAgent(actionBuffers);
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity += rBody.velocity;
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
        
        // actionsOut.ContinuousActions.Array[0] = Input.GetAxis("Mouse X");
        actionsOut.ContinuousActions.Array[0] = Input.GetKey(KeyCode.Space) ? 1f : 0f;
    }
}