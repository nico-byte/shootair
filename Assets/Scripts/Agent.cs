using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Linq;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;
using Unity.Collections;
using UnityEngine.SocialPlatforms;
using System.Runtime.CompilerServices;

namespace ShootAirRLAgent
{

    public enum PlayerState
    {
        Idle,
        Moving,
        Aiming,
        MovingAiming
    }

    public class ShootairAgent : Agent
    {
        // VARIABLES
        Rigidbody2D rBody;
        AgentSettings agentSettings;
        BufferSensorComponent bufferSensor;
        private Animator anim;
        [SerializeField]
        private GameObject bulletPrefab;
        [SerializeField]
        private Transform firingPoint;

        EnvironmentController envController;
        AgentObservations agentObservations;

        private Vector2 trackVelocity;
        private Vector2 lastPos;

        private bool shotAvailable;

        private bool stateLock;
        private PlayerState playerState;

        void Start()
        {
            envController = FindObjectOfType<EnvironmentController>();
            agentObservations = FindObjectOfType<AgentObservations>();
            anim = gameObject.GetComponent<Animator>();

        }


        public override void Initialize()
        {
            base.Initialize();
            rBody = GetComponent<Rigidbody2D>();
            agentSettings = FindObjectOfType<AgentSettings>();
            bufferSensor = gameObject.GetComponent<BufferSensorComponent>();
            stateLock = false;
            playerState = PlayerState.Idle;
        }

        void FixedUpdate()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            if (agentSettings.selfplay)
            {
                MoveAgent(ActionBuffers.Empty);
            }

            trackVelocity = (rBody.position - lastPos) * 50;
            lastPos = rBody.position;

            shotAvailable = agentSettings.fireTimer <= 0f;
            agentSettings.fireTimer -= Time.deltaTime;
        }

        void OnCollisionEnter2D(Collision2D c)
        {
            if (c.gameObject.CompareTag("target"))
            {
                envController.ResolveEvent(Event.collisionWithTarget);
            }
        }

        private void SetState(PlayerState newState)
        {
            if (!stateLock)
            {
                playerState = newState;
                switch (playerState)
                {
                    case PlayerState.Idle:
                        anim.Play("Idle");
                        break;
                    case PlayerState.Moving:
                        anim.Play("Moving");
                        break;
                    case PlayerState.Aiming:
                        anim.Play("Aiming");
                        break;
                    case PlayerState.MovingAiming:
                        anim.Play("MovingAiming");
                        break;
                }
            }
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // Agent velocity
            sensor.AddObservation(trackVelocity.x / agentSettings.maxVelocity);
            agentObservations.observations["velocity_x"] = trackVelocity.x / agentSettings.maxVelocity;
            sensor.AddObservation(trackVelocity.y / agentSettings.maxVelocity);
            agentObservations.observations["velocity_y"] = trackVelocity.y / agentSettings.maxVelocity;

            // shotAvailable
            sensor.AddObservation(shotAvailable ? 1 : 0);
            agentObservations.observations["shotAvailable"] = shotAvailable ? 1 : 0;


            // Surrounding enemies
            // Collect observation about the 10 closest enemies
            var enemies = envController.EnemyList.ToArray();
            
            // sensor.AddObservation(enemies.Length);
            // agentObservations.observations["enemiesLeft"] = enemies.Length;

            // Sort by closest :
            enemies = enemies.Where(e => e != null && e.activeInHierarchy).ToArray();
            Array.Sort(enemies, (a, b) => Vector3.Distance(a.transform.position, transform.position).CompareTo(Vector3.Distance(b.transform.position, transform.position)));
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

                float distance = Vector2.Distance(this.transform.position, b.transform.position);
                distance /= agentSettings.maxDistance;
                float direction = Vector2.SignedAngle(this.transform.position, b.transform.position) / 180f;
                // float timeLeft = envController.scaleTimer;
                // timeLeft /= envController.desiredLength;
                Vector2 velocity = enemy.trackVelocity / agentSettings.maxVelocity;
                float health = enemy.health;
                health /= agentSettings.maxHealth;

                float[] enemyObservation = new float[]{
                    distance,
                    direction,
                    velocity.x,
                    velocity.y,
                    // timeLeft,
                    health
                };
                numEnemyAdded += 1;

                if (numEnemyAdded == 1)
                {
                    agentObservations.observations["distanceEnemy"] = distance;
                    agentObservations.observations["directionEnemy"] = direction;
                    agentObservations.observations["velocity_xEnemy"] = velocity.x;
                    agentObservations.observations["velocity_yEnemy"] = velocity.y;
                    // agentObservations.observations["timeLeft"] = timeLeft;
                    agentObservations.observations["healthEnemy"] = health;

                }

                bufferSensor.AppendObservation(enemyObservation);
            }
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
            if (amountSpeed == 0)
            {
                SetState(PlayerState.Idle);
                anim.SetBool("IsMoving", false);
            }

            bool shootingStar = shootingStates.Values.Any(c => c);
            if (shootingStar && playerState == PlayerState.Idle)
            {
                SetState(PlayerState.Aiming);
                anim.SetBool("IsMoving", false);
            }

            // Movement and shooting
            float forwardAmount = 0f;
            float turnAmount = 0f;

            foreach (var direction in directionStates.Keys)
            {
                if (directionStates[direction])
                {
                    SetState(PlayerState.Moving);
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

            FireWeapon(shootingStates, shootingStar);
            
            anim.SetFloat("xMove", turnAmount);
            anim.SetFloat("yMove", forwardAmount);
            anim.SetBool("IsMoving", true);

            float diagionalSpeed = amountSpeed > 1 ? 0.707106781f : 1f;
            Vector3 movementForward = agentSettings.moveSpeed * forwardAmount * Time.fixedDeltaTime * transform.up * diagionalSpeed;
            Vector3 movementTurn = agentSettings.moveSpeed * turnAmount * Time.fixedDeltaTime * transform.right * diagionalSpeed;
            Vector3 movement = movementForward + movementTurn;
            rBody.MovePosition(transform.position + movement);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            if (!agentSettings.selfplay)
            {
                MoveAgent(actions);
            }
        }

        private void FireWeapon(Dictionary<string, bool> shootingStates = null, bool shootingStar = false)
        {
            foreach (var rotation in shootingStates.Keys)
            {
                if (shootingStates[rotation] && shotAvailable && shootingStar)
                {
                    // Only update the firing direction if the agent is not currently firing
                    // Change firingPoint rotation based on shooting key pressed
                    SetState(PlayerState.MovingAiming);
                    float shootingRotation;
                    if (rotation == "Up" || rotation == "Down")
                    {
                        shootingRotation = rotation == "Up" ? 0f : 180f;
                    }
                    else
                    {
                        shootingRotation = rotation == "Left" ? 90f : 270f;
                    }

                    switch (agentSettings.weaponEquipped)
                    {
                        case "pistol": // Pistol Equipped
                            Shoot(shootingRotation, speed: 15f, damage: 45, lifetime: 3f, bulletAmount: 1, bulletSpread: 0f);
                            agentSettings.fireTimer = 0.5f;
                            break;
                        case "rifle": // Rifle Equipped
                            Shoot(shootingRotation, speed: 25f, damage: 20, lifetime: 3f, bulletAmount: 1, bulletSpread: 2f);
                            agentSettings.fireTimer = 0.18f;
                            break;
                        case "shotgun": // Shotgun Equipped
                            Shoot(shootingRotation, speed: 12f, damage: 8, lifetime: 0.8f, bulletAmount: 20, bulletSpread: 15f);
                            agentSettings.fireTimer = 1.2f;
                            break;
                    }

                    // Reset ShotAvailable
                    shotAvailable = false;
                }
            }
        }

        private void Shoot(float rotation, float speed = 20f, int damage = 40, float lifetime = 3f, int bulletAmount = 1, float bulletSpread = 0f)
        {
            for (int i = 0; i < bulletAmount; i++)
            {
                float rotationOffset = bulletSpread == 0f ? 0f : UnityEngine.Random.Range(-bulletSpread, bulletSpread);
                firingPoint.transform.rotation = Quaternion.Euler(0f, 0f, rotation + rotationOffset);
                GameObject bullet = Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation);
                bullet.GetComponent<Bullet>().bulletSettings(speed, damage, lifetime);
            }
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
}