using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Linq;
using System;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
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
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("target"))
        {
            envController.ResolveEvent(Event.collisionWithTarget);
        }

        /*
        if (c.gameObject.CompareTag("wall"))
		{
            envController.ResolveEvent(Event.hitWall);
		}
        */
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agent position
        // sensor.AddObservation(this.transform.localPosition);

        // Agent rotation
        // sensor.AddObservation(this.transform.localRotation);

        // Firing point position and rotation
        // sensor.AddObservation(firingPoint.localPosition);
        sensor.AddObservation(firingPoint.localRotation);

        // Agent velocity
        // sensor.AddObservation(rBody.velocity.x);
        // sensor.AddObservation(rBody.velocity.y);

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
            Rigidbody2D bRigid = b.GetComponent<Rigidbody2D>();

            float distance = (b.transform.position - transform.position).sqrMagnitude;

            float[] enemyObservation = new float[]{
                distance,
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
            shootingStates["Down"] = actionBuffers.DiscreteActions[3] == 1;
            shootingStates["Left"] = actionBuffers.DiscreteActions[4] == 1;
            shootingStates["Right"] = actionBuffers.DiscreteActions[5] == 1;
        }
    
        int amountSpeed = directionStates.Values.Count(c => c);
        int amountShoot = shootingStates.Values.Count(b => b);
    
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
                if (rotation == "Up" || rotation == "Down")
                {
                    firingPoint.transform.rotation = rotation == "Up" ? UnityEngine.Quaternion.Euler(0f, 0f, 0f) : UnityEngine.Quaternion.Euler(0, 0, 180f);
                }
                else
                {
                    firingPoint.transform.rotation = rotation == "Left" ? UnityEngine.Quaternion.Euler(0f, 0f, 90f) : UnityEngine.Quaternion.Euler(0, 0, 270f);
                }
            }
        }
    
        // Shooting
        if (amountShoot >= 1)
        {
            if (agentSettings.fireTimer <= 0f)
            {
                Shoot();
                agentSettings.fireTimer = agentSettings.fireRate;
            }
            else
            {
                agentSettings.fireTimer -= Time.deltaTime;
            }
        }
        else
        {
            if (agentSettings.autoShoot && agentSettings.fireTimer <= 0f)
            {
                Shoot();
                agentSettings.fireTimer = agentSettings.fireRate;
            }
            else
            {
                agentSettings.fireTimer -= Time.deltaTime;
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

        // Put the actions into the array
        actionsOut.DiscreteActions.Array[0] = forwardAction;
        actionsOut.DiscreteActions.Array[1] = turnAction;

        actionsOut.DiscreteActions.Array[2] = Input.GetKey(KeyCode.UpArrow) ? 1 : 0;
        actionsOut.DiscreteActions.Array[3] = Input.GetKey(KeyCode.DownArrow) ? 1 : 0;
        actionsOut.DiscreteActions.Array[4] = Input.GetKey(KeyCode.LeftArrow) ? 1 : 0;
        actionsOut.DiscreteActions.Array[5] = Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
    }
}