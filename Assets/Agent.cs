using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class ShootairAgent : Agent
{
    Rigidbody2D rBody;
    AgentSettings agentSettings;
    // Gun properties
    public GameObject bulletPrefab;
    public GameObject enemyPrefab;
    public Transform firingPoint;
    public int numberOfTargets = 8;
    private int enemiesInit;
    public Transform Enemy;
    public Transform Bullet;
    public int warmup = 50000;
    
    public override void Initialize()
    {
        base.Initialize();
        rBody = GetComponent<Rigidbody2D>();
        agentSettings = FindObjectOfType<AgentSettings>();
    }

    void Update()
    {
         Cursor.visible = false;
         Cursor.lockState = CursorLockMode.Locked;
    }
    
    public override void OnEpisodeBegin()
    {
        if (CompletedEpisodes < warmup)
        {
            MaxStep = 1000;
        } else MaxStep = 10000;
        
        enemiesInit = numberOfTargets * 2;
        
        this.rBody.angularVelocity = 0;
        this.rBody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(0, 0, 0);
        
        // kill previous instances of enemies
        Object[] allObjects = FindObjectsOfType(typeof(GameObject));
        foreach(GameObject obj in allObjects) {
            if(obj.transform.name == "Enemy(Clone)"){
                Destroy(obj);
            }
        }

        // Move the target to a new spot
        for (int i = 0; i <= numberOfTargets; i++)
        {
            Instantiate(enemyPrefab, new Vector3(Random.value * 12 + 4, Random.value * 8 - 4, 0f), Quaternion.Euler(0f, 0f, Random.Range(0.0f, 360.0f)));
            Instantiate(enemyPrefab, new Vector3(Random.value * - 12 - 4, Random.value * 8 - 4, 0f), Quaternion.Euler(0f, 0f, Random.Range(0.0f, 360.0f)));
        }
        if (CompletedEpisodes == warmup)
        {
            Debug.Log("Warmup Phase now over. Some Rewards are cut out of the training process.");
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

    /// Moves the agent according to the selected action.
    /// </summary>
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (GameObject.Find("Enemy(Clone)") == null)
        {
            AddReward(1f-GetCumulativeReward());
            Debug.Log("Episode " + CompletedEpisodes + " completed with reward: " + GetCumulativeReward());
            EndEpisode();
        }
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