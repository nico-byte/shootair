using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Linq;
using System;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Numerics;
//using Unity.MLAgents.Policies;
//using Unity.VisualScripting.Dependencies.Sqlite;
//using UnityEngine.AI;

public class ShootairAgent : Agent
{
    // VARIABLES
    Rigidbody2D rBody;
    AgentSettings agentSettings;
    BufferSensorComponent bufferSensor;
    private Animator anim;
    public GameObject bulletPrefab;
    public Transform firingPoint;

    EnvironmentController envController;

    private UnityEngine.Vector2 trackVelocity;
    private UnityEngine.Vector2 lastPos;

    private bool shotAvailable;

    //BehaviorParameters behaviorParameters;
    //EnvironmentParameters resetParams;    

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
        //behaviorParameters = gameObject.GetComponent<BehaviorParameters>();
        //resetParams = Academy.Instance.EnvironmentParameters;
        bufferSensor = gameObject.GetComponent<BufferSensorComponent>();
    }

    void FixedUpdate()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if(agentSettings.selfplay) {
            MoveAgent(ActionBuffers.Empty);
        }

        trackVelocity = (rBody.position - lastPos) * 50;
        lastPos = rBody.position;

        shotAvailable = agentSettings.fireTimer <= 0f ? true : false;
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
        // Agent velocity
        sensor.AddObservation(trackVelocity.x / agentSettings.maxVelocity);
        sensor.AddObservation(trackVelocity.y / agentSettings.maxVelocity);

        // shotAvailable
        sensor.AddObservation(agentSettings.fireTimer / agentSettings.fireRate);
        sensor.AddObservation(shotAvailable ? 1 : 0);

        // Surrounding enemies
        // Collect observation about the 20 closest enemies
        var enemies = envController.EnemyList.ToArray();
        // Sort by closest :
        enemies = enemies.Where(e => e != null && e.activeInHierarchy).ToArray();
        Array.Sort(enemies, (a, b) => UnityEngine.Vector3.Distance(a.transform.position, transform.position).CompareTo(UnityEngine.Vector3.Distance(b.transform.position, transform.position)));
        int numEnemyAdded = 0;
        
        foreach (GameObject b in enemies)
        {
            if (numEnemyAdded >= 5)
            {
                break;
            }

            if (b == null || !b.activeInHierarchy)
            {
                continue;
            }

            EnemyAI enemy = b.GetComponent<EnemyAI>();

            float distance = this.transform.InverseTransformVector(b.transform.position - transform.position).sqrMagnitude;
            float direction = UnityEngine.Vector3.SignedAngle(this.transform.forward, b.transform.position - this.transform.position, UnityEngine.Vector3.up) / 180f;
            UnityEngine.Vector2 velocity = enemy.trackVelocity / agentSettings.maxVelocity;

            float[] enemyObservation = new float[]{
                distance,
                direction,
                velocity.x,
                velocity.y,
                enemy.health / 150f
            };
            numEnemyAdded += 1;

            bufferSensor.AppendObservation(enemyObservation);
        };
    }

    public void MoveAgent(ActionBuffers actionBuffers)
    {
        Dictionary<string, bool> directionStates = new Dictionary<string, bool>
        {
            {"Up", false},
            {"Down", false},
            {"Left", false},
            {"Right", false}
        };
    
        Dictionary<string, bool> shootingStates = new Dictionary<string, bool>
        {
            {"Up", false},
            {"Down", false},
            {"Left", false},
            {"Right", false}
        };
    
        if (agentSettings.selfplay)
        {
            directionStates["Up"] = Input.GetKey(KeyCode.W);
            directionStates["Down"] = Input.GetKey(KeyCode.S);
            directionStates["Left"] = Input.GetKey(KeyCode.A);
            directionStates["Right"] = Input.GetKey(KeyCode.D);
    
            shootingStates["Up"] = Input.GetKey(KeyCode.UpArrow);
            shootingStates["Down"] = Input.GetKey(KeyCode.DownArrow);
            shootingStates["Left"] = Input.GetKey(KeyCode.LeftArrow);
            shootingStates["Right"] = Input.GetKey(KeyCode.RightArrow);
        }
        else
        {
            directionStates["Up"] = actionBuffers.DiscreteActions[0] == 1;
            directionStates["Down"] = actionBuffers.DiscreteActions[0] == 2;
            directionStates["Left"] = actionBuffers.DiscreteActions[1] == 1;
            directionStates["Right"] = actionBuffers.DiscreteActions[1] == 2;

            shootingStates["Up"] = actionBuffers.DiscreteActions[2] == 1;
            shootingStates["Down"] = actionBuffers.DiscreteActions[2] == 2;
            shootingStates["Left"] = actionBuffers.DiscreteActions[2] == 3;
            shootingStates["Right"] = actionBuffers.DiscreteActions[2] == 4;
        }
    
        int amountSpeed = directionStates.Values.Count(c => c);
        bool shootingStar = shootingStates.Values.Any(c => c) ? true : false;
    
        // Set animation states
        foreach (var direction in directionStates.Keys)
        {
            anim.SetBool($"walking{direction}", directionStates[direction]);
        }
    
        // Movement and shooting
        float forwardAmount = 0f;
        float turnAmount = 0f;
    
        foreach (var direction in directionStates.Keys)
        {
            if (directionStates[direction])
            {
                if (direction == "Up" || direction == "Down")
                {
                    forwardAmount = direction == "Up" ? 1f : -1f;
                }
                else
                {
                    turnAmount = direction == "Right" ? 1f : -1f;
                }
            }
        }

        foreach (var rotation in shootingStates.Keys)
        {
            if (shootingStates[rotation])
            {
                // Only update the firing direction if the agent is not currently firing
                if (shotAvailable && shootingStar)
                {
                    if (rotation == "Up" || rotation == "Down")
                    {
                        firingPoint.transform.rotation = rotation == "Up" ? UnityEngine.Quaternion.Euler(0f, 0f, 0f) : UnityEngine.Quaternion.Euler(0, 0, 180f);
                    }
                    else
                    {
                        firingPoint.transform.rotation = rotation == "Left" ? UnityEngine.Quaternion.Euler(0f, 0f, 90f) : UnityEngine.Quaternion.Euler(0, 0, 270f);
                    }
                    
                    Shoot();
                    agentSettings.fireTimer = agentSettings.fireRate;
                    shotAvailable = false;
                }
                else
                {
                    agentSettings.fireTimer -= Time.deltaTime;
                }
            }
        }
    
        float diagionalSpeed = amountSpeed > 1 ? 0.707106781f : 1f;
        UnityEngine.Vector3 movementForward = agentSettings.moveSpeed * forwardAmount * Time.fixedDeltaTime * transform.up * diagionalSpeed;
        UnityEngine.Vector3 movementTurn = agentSettings.moveSpeed * turnAmount * Time.fixedDeltaTime * transform.right * diagionalSpeed;
        UnityEngine.Vector3 movement = movementForward + movementTurn;
        rBody.MovePosition(transform.position + movement);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (!agentSettings.selfplay) {
            MoveAgent(actionBuffers);
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation);
        bullet.transform.parent = firingPoint;
        // bullet.GetComponent<Rigidbody2D>().velocity += rBody.velocity;
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
            // move backwards
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

        if (Input.GetKey(KeyCode.UpArrow))
        {
            // shoot
            shootAction = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            // shoot
            shootAction = 2;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            // shoot
            shootAction = 3;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            // shoot
            shootAction = 4;
        }

        // Put the actions into the array
        actionsOut.DiscreteActions.Array[0] = forwardAction;
        actionsOut.DiscreteActions.Array[1] = turnAction;

        actionsOut.DiscreteActions.Array[2] = shootAction;
    }

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        if (!shotAvailable)
        {
            actionMask.SetActionEnabled(2, 1, false);
            actionMask.SetActionEnabled(2, 2, false);
            actionMask.SetActionEnabled(2, 3, false);
            actionMask.SetActionEnabled(2, 4, false);
        }
    }
}