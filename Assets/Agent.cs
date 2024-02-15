using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.VisualScripting;
using Unity.Sentis.Layers;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;

public class ShootairAgent : Agent
{
    Rigidbody2D rBody;
    public float moveSpeed = 5f;

    public float turnSpeed = 360f;

    // Gun properties
    public bool autoShoot = false;
    public GameObject bulletPrefab;
    public GameObject enemyPrefab;
    public Transform firingPoint;
    public float fireRate = 0.5f;
    public int numberOfTargets = 8;

    public float fireTimer;
    private int lastRewardExp;
    private int enemiesInit;
    public Transform Enemy;
    public Transform FiringPoint;
    public Transform NorthWall;
    public Transform SouthWall;
    public Transform WestWall;
    public Transform EastWall;
    public Transform Bullet;
    public int warmup = 50000;
    private bool hasShot = false;
    public override void Initialize()
    {
        base.Initialize();
        rBody = GetComponent<Rigidbody2D>();
        // Hides the cursor...
    }

    
    public override void OnEpisodeBegin()
    {
        if (CompletedEpisodes < warmup)
        {
            MaxStep = 1000;
        } else MaxStep = 10000;

        hasShot = false;
        
        lastRewardExp = 0;
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

    void Update()
    {
         Cursor.visible = false;
         Cursor.lockState = CursorLockMode.Locked;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Enemy.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        
        // Agent rotation
        sensor.AddObservation(this.transform.rotation);

        // Firing point position and rotation
        sensor.AddObservation(FiringPoint.position);
        sensor.AddObservation(FiringPoint.rotation);

        // distance to enemies
        float distanceToEnemies = Vector2.Distance(this.transform.localPosition, Enemy.localPosition);
        sensor.AddObservation(distanceToEnemies);

        // Walls
        float distanceToNorthWall = Vector2.Distance(this.transform.localPosition, NorthWall.localPosition);
        float distanceToSouthWall = Vector2.Distance(this.transform.localPosition, SouthWall.localPosition);
        float distanceToWestWall = Vector2.Distance(this.transform.localPosition, WestWall.localPosition);
        float distanceToEasthWall = Vector2.Distance(this.transform.localPosition, EastWall.localPosition);
        sensor.AddObservation(distanceToNorthWall);
        sensor.AddObservation(distanceToSouthWall);
        sensor.AddObservation(distanceToWestWall);
        sensor.AddObservation(distanceToEasthWall);

        // Bullet
        // sensor.AddObservation(Bullet.position);
        // sensor.AddObservation(Bullet.rotation);
        float distanceBulletEnemy = Vector2.Distance(Bullet.position, Enemy.position);
        sensor.AddObservation(distanceBulletEnemy);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.y);
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
        
        // Convert the first action to forward movement
        float forwardAmount = 0f;       
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
        {;
                hasShot = true;
            turnAmount = -1f;
        }
        else if (actionBuffers.DiscreteActions[1] == 2f)
        {
            turnAmount = 1f;
        }
        if ((actionBuffers.ContinuousActions[1] == 1 && fireTimer <= 0f) || (autoShoot && fireTimer <= 0f))
        {
            Shoot();
            if (!autoShoot)
            {
                AddReward(0.3f/(MaxStep*fireRate));
                hasShot = true;
                Debug.Log("Shoot!");
            }
            fireTimer = fireRate;
        } else
        {
            fireTimer -= Time.deltaTime;
        }

        // Apply movement
        rBody.MovePosition(transform.position + transform.up * forwardAmount * moveSpeed * Time.fixedDeltaTime + transform.right * turnAmount * moveSpeed * Time.fixedDeltaTime);
        transform.Rotate(transform.forward * -actionBuffers.ContinuousActions[0] * turnSpeed * Time.fixedDeltaTime);

        // Apply a tiny negative reward every step to encourage action
        if (MaxStep > 0 && fireTimer < -5f) AddReward(-1f / MaxStep);

        float distanceBulletEnemy = Vector2.Distance(Bullet.position, Enemy.position);
        if (distanceBulletEnemy <= 1.64f && CompletedEpisodes < warmup) AddReward(0.2f / (fireRate*MaxStep));

        int leftEnemies = GameObject.FindGameObjectsWithTag("target").Length;
        int rewardExp = enemiesInit - leftEnemies;
        

        if (rewardExp > lastRewardExp)
        {
            AddReward(0.5f / enemiesInit);
            lastRewardExp = rewardExp;
        }

        if (StepCount == MaxStep)
        {
            AddReward(-0.3f);
            Debug.Log("Episode " + CompletedEpisodes + " expired with reward: " + GetCumulativeReward());
            EndEpisode();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "target")
        {
            if (hasShot) AddReward(-0.3f);
            else AddReward(-1f-GetCumulativeReward());
            Debug.Log("Episode " + CompletedEpisodes + " canceled with reward: " + GetCumulativeReward());
            EndEpisode();
        }
        if (collision.transform.tag == "wall")
        {
            if (CompletedEpisodes < warmup) 
            {
                if (hasShot) AddReward(-0.3f);
                else AddReward(-1f-GetCumulativeReward());
                Debug.Log("Episode " + CompletedEpisodes + " canceled with reward: " + GetCumulativeReward());
                EndEpisode();
            } else AddReward(-1f/MaxStep);
        }
    }
    private void Shoot()
    {
        Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int forwardAction = 0;
        int turnAction = 0;
        int shootAction = 0;
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
        if (Input.GetKey(KeyCode.Space))
        {
            // turn right
            shootAction = 1;
        }

        // Put the actions into the array
        actionsOut.DiscreteActions.Array[0] = forwardAction;
        actionsOut.DiscreteActions.Array[1] = turnAction;
        // actionsOut.DiscreteActions.Array[2] = shootAction;
        actionsOut.ContinuousActions.Array[0] = Input.GetAxisRaw("Mouse X");
        actionsOut.ContinuousActions.Array[1] = shootAction;
    }
}