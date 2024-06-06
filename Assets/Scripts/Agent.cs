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
using System.Collections;
using Google.Protobuf.WellKnownTypes;

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
        SoundEffectPlayer soundHandler;
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

        private bool currentlyWalking = false;
        private float soundRatelimit = 0f;

        private bool shotAvailable;
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
            soundHandler = FindObjectOfType<SoundEffectPlayer>();
            bufferSensor = gameObject.GetComponent<BufferSensorComponent>();
            playerState = PlayerState.Idle;
        }

        void FixedUpdate()
        {
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

            playerState = newState;
            switch (playerState)
            {
                case PlayerState.Idle:
                    //anim.Play("Idle");
                    break;
                case PlayerState.Moving:
                    //anim.Play("Moving");
                    break;
                case PlayerState.Aiming:
                    //anim.Play("Aiming");
                    break;
                case PlayerState.MovingAiming:
                    //anim.Play("MovingAiming");
                    break;
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
            // Collect observation about the 20 closest enemies
            var enemies = envController.EnemyList.ToArray();
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
                float timeLeft = envController.scaleTimer;
                timeLeft /= envController.desiredLength;
                float health = enemy.health;
                health /= agentSettings.maxHealth;

                float[] enemyObservation = new float[]{
                    distance,
                    direction,
                    timeLeft,
                    health
                };
                numEnemyAdded += 1;

                if (numEnemyAdded == 1)
                {
                    agentObservations.observations["distanceEnemy"] = distance;
                    agentObservations.observations["directionEnemy"] = direction;
                    agentObservations.observations["timeLeft"] = timeLeft;
                    agentObservations.observations["healthEnemy"] = health;

                }

                bufferSensor.AppendObservation(enemyObservation);
            }
        }

        public void MoveAgent(ActionBuffers actionBuffers)
        {
            Debug.Log("IsAiming: " + anim.GetBool("IsAiming"));
            Debug.Log("IsMoving: " + anim.GetBool("IsMoving"));

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
                //SetState(PlayerState.Idle);
                anim.SetBool("IsMoving", false);
            } else {
                //SetState(PlayerState.Moving);
                anim.SetBool("IsMoving", true);
            }

            // AGENT STEP SOUND
            if (amountSpeed == 0)
            {
                currentlyWalking = false;
            }
            else if (amountSpeed >= 1)
            {
                if (!currentlyWalking)
                {
                    currentlyWalking = true;
                }
                else
                {
                    //play step sound
                    if (soundRatelimit <= 0f)
                    {
                        soundHandler.playSound("agent_ambient", 0.2f, 0.2f);
                        soundRatelimit = 0.275f;
                    }
                    else
                    {
                        soundRatelimit -= Time.deltaTime;
                    }
                }
            }

            bool shootingStar = shootingStates.Values.Any(c => c);
            //if (shootingStar && playerState == PlayerState.Idle)
            //{
            //    //SetState(PlayerState.Aiming);
            //}
            if (!shootingStar)
            {
                SetWeaponSprites(false, false, false, false);
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

            FireWeapon(shootingStates, shootingStar);

            anim.SetFloat("xMove", turnAmount);
            anim.SetFloat("yMove", forwardAmount);

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

        private void SetWeaponSprites(bool up, bool down, bool left, bool right)
        {
            anim.SetBool("IsAiming", true);
            Transform weapons = transform.Find("weapons");
            GameObject weaponUp = weapons.Find(agentSettings.weaponEquipped + "Up").gameObject;
            GameObject weaponDown = weapons.Find(agentSettings.weaponEquipped + "Down").gameObject;
            GameObject weaponLeft = weapons.Find(agentSettings.weaponEquipped + "Left").gameObject;
            GameObject weaponRight = weapons.Find(agentSettings.weaponEquipped + "Right").gameObject;
            weaponUp.SetActive(up);
            weaponDown.SetActive(down);
            weaponLeft.SetActive(left);
            weaponRight.SetActive(right);
        }

        private void FireWeapon(Dictionary<string, bool> shootingStates = null, bool shootingStar = false)
        {
            foreach (var rotation in shootingStates.Keys)
            {
                //Debug.Log(rotation);
                if (shootingStates[rotation] && shotAvailable && shootingStar)
                {
                    // Only update the firing direction if the agent is not currently firing
                    // Change firingPoint rotation based on shooting key pressed
                    //SetState(PlayerState.Aiming);
                    float shootingRotation;
                    if (rotation == "Up" || rotation == "Down")
                    {
                        shootingRotation = rotation == "Up" ? 0f : 180f;
                    }
                    else
                    {
                        shootingRotation = rotation == "Left" ? 90f : 270f;
                    }
                    anim.SetFloat("AimingDirection",shootingRotation);

                    switch (shootingRotation)
                    {
                        case 0f:
                            SetWeaponSprites(true, false, false, false);
                            break;
                        case 90f:
                            SetWeaponSprites(false, false, true, false);
                            break;
                        case 180f:
                            SetWeaponSprites(false, true, false, false);
                            break;
                        case 270f:
                            SetWeaponSprites(false, false, false, true);
                            break;
                    }

                    switch (agentSettings.weaponEquipped)
                    {
                        case "pistol": // Pistol Equipped
                            Shoot(shootingRotation, speed: 20f, damage: 45, lifetime: 3f, bulletAmount: 1, bulletSpread: 0f);
                            soundHandler.playSound("weapon_pistol_shoot");
                            agentSettings.fireTimer = 0.4f;
                            break;
                        case "rifle": // Rifle Equipped
                            Shoot(shootingRotation, speed: 25f, damage: 20, lifetime: 3f, bulletAmount: 1, bulletSpread: 0f);
                            soundHandler.playSound("weapon_rifle_shoot");
                            agentSettings.fireTimer = 0.18f;
                            break;
                        case "shotgun": // Shotgun Equipped
                            Shoot(shootingRotation, speed: 12f, damage: 9, lifetime: 1.3f, bulletAmount: 12, bulletSpread: 15f);
                            soundHandler.playSound("weapon_shotgun_shoot");
                            agentSettings.fireTimer = 1.2f;
                            break;
                    }

                    // Reset ShotAvailable
                    shotAvailable = false;
                } else if (!shootingStar) {
                    anim.SetBool("IsAiming",false);
                }
            }
        }
        private IEnumerator PlayParticleEffect(GameObject particleShot)
        {
            ParticleSystem ps = particleShot.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop();
                ps.Clear();
            }

            particleShot.SetActive(true);

            if (ps != null)
            {
                ps.Play();
            }

            yield return new WaitForSeconds(1f);

            if (ps != null)
            {
                ps.Stop();
            }
            particleShot.SetActive(false);
        }

        private void Shoot(float rotation, float speed = 20f, int damage = 40, float lifetime = 3f, int bulletAmount = 1, float bulletSpread = 0f)
        {
            // needs fix to play multiple times when shooting
            Transform weapons = transform.Find("weapons");
            GameObject particleShot = weapons.Find("shot").gameObject;
            StartCoroutine(PlayParticleEffect(particleShot));

            for (int i = 0; i < bulletAmount; i++)
            {
                float rotationOffset = bulletSpread == 0f ? 0f : UnityEngine.Random.Range(-bulletSpread, bulletSpread);
                float speedOffset = bulletSpread == 0f ? 0f : UnityEngine.Random.Range(-speed * 0.15f, speed * 0.15f);
                firingPoint.transform.rotation = Quaternion.Euler(0f, 0f, rotation + rotationOffset);
                GameObject bullet = Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation);
                bullet.GetComponent<Bullet>().bulletSettings(speed + speedOffset, damage, lifetime);
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