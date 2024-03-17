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
    private Animator anim;
    public GameObject bulletPrefab;
    public Transform firingPoint;

    EnvironmentController envController;

    EnvironmentParameters resetParams;    
    
    void Start()
    {
        envController = FindObjectOfType<EnvironmentController>();
        anim = gameObject.GetComponent<Animator>();
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
 
        // Check for space key
        switch (Input.GetKey(KeyCode.Space))
        {
            case true:
                anim.SetBool("aiming", true);
                break;
            default:
                anim.SetBool("aiming", false);
                break;
        }

        // Check for directional keys
        bool walkingUp = Input.GetKey(KeyCode.W);
        bool walkingDown = Input.GetKey(KeyCode.S);
        bool walkingLeft = Input.GetKey(KeyCode.A);
        bool walkingRight = Input.GetKey(KeyCode.D);

        // Set animation bools based on directional keys
        switch (walkingUp)
        {
            case var value when value == true:
                anim.SetBool("walkingUp", true);
                break;
            default:
                anim.SetBool("walkingUp", false);
                break;
        }

        switch (walkingDown)
        {
            case var value when value == true:
                anim.SetBool("walkingDown", true);
                break;
            default:
                anim.SetBool("walkingDown", false);
                break;
        }

        switch (walkingLeft)
        {
            case var value when value == true:
                anim.SetBool("walkingLeft", true);
                break;
            default:
                anim.SetBool("walkingLeft", false);
                break;
        }

        switch (walkingRight)
        {
            case var value when value == true:
                anim.SetBool("walkingRight", true);
                break;
            default:
                anim.SetBool("walkingRight", false);
                break;
        }
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
                anim.SetBool("aiming", true);
                forwardAmount = transform.eulerAngles.z == 0 ? 1f : -1f;
            }
            else if (actionBuffers.ContinuousActions[0] == 1 && (transform.eulerAngles.z == 90 || transform.eulerAngles.z == 270))
            {
                anim.SetBool("aiming", true);
                turnAmount = transform.eulerAngles.z == 90 ? 1f : -1f;
            }
            else if (actionBuffers.ContinuousActions[0] != 1)
            {
                anim.SetBool("aiming", false);
                forwardAmount = 1f;
                firingPoint.transform.rotation = Quaternion.Euler(0, 0, 0f);
            }
        }
        else if (actionBuffers.DiscreteActions[0] == 2f)
        {            
            if (actionBuffers.ContinuousActions[0] == 1 && (transform.eulerAngles.z == 0 || transform.eulerAngles.z == 180))
            {
                anim.SetBool("aiming", true);
                forwardAmount = transform.eulerAngles.z == 0 ? -1f : 1f;
            }
            else if (actionBuffers.ContinuousActions[0] == 1 && (transform.eulerAngles.z == 90 || transform.eulerAngles.z == 270))
            {
                anim.SetBool("aiming", true);
                turnAmount = transform.eulerAngles.z == 90 ? -1f : 1f;
            }
            else if (actionBuffers.ContinuousActions[0] != 1)
            {
                anim.SetBool("aiming", false);
                forwardAmount = -1f;
                firingPoint.transform.rotation = Quaternion.Euler(0, 0, 180f);
            }
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
                turnAmount = -1f;
                firingPoint.transform.rotation = Quaternion.Euler(0, 0, 90f);
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
                turnAmount = 1f;
                firingPoint.transform.rotation = Quaternion.Euler(0, 0, 270f);
            }
        }

        // Apply movement
        Vector3 movementForward = transform.up * forwardAmount * agentSettings.moveSpeed * Time.fixedDeltaTime;
        Vector3 movementTurn = transform.right * turnAmount * agentSettings.moveSpeed * Time.fixedDeltaTime;
        Vector3 movement = movementForward + movementTurn;
        rBody.MovePosition(transform.position + movement);
        
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
        
        actionsOut.ContinuousActions.Array[0] = Input.GetKey(KeyCode.Space) ? 1f : 0f;
    }
}