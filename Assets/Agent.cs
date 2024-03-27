using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Linq;
using System;
using Unity.VisualScripting;
//using Unity.MLAgents.Policies;
//using Unity.VisualScripting.Dependencies.Sqlite;
//using UnityEngine.AI;

public class ShootairAgent : Agent
{
    // VARIABLES
    Rigidbody2D rBody;
    AgentSettings agentSettings;
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
        // Check for directional keys
        bool walkingUp = false;
        bool walkingDown = false;
        bool walkingLeft = false;
        bool walkingRight = false;
        bool upArrow = false;
        bool downArrow = false;
        bool leftArrow = false;
        bool rightArrow = false;
        
        if (agentSettings.selfplay)
        {
            walkingUp = Input.GetKey(KeyCode.W);
            walkingDown = Input.GetKey(KeyCode.S);
            walkingLeft = Input.GetKey(KeyCode.A);
            walkingRight = Input.GetKey(KeyCode.D);
            upArrow = Input.GetKey(KeyCode.UpArrow);
            downArrow = Input.GetKey(KeyCode.DownArrow);
            leftArrow = Input.GetKey(KeyCode.LeftArrow);
            rightArrow = Input.GetKey(KeyCode.RightArrow);
        }
        else
        {
            walkingUp = actionBuffers.DiscreteActions[0] == 1;
            walkingDown = actionBuffers.DiscreteActions[0] == 2;
            walkingLeft = actionBuffers.DiscreteActions[1] == 1;
            walkingRight = actionBuffers.DiscreteActions[1] == 2;
            upArrow = actionBuffers.DiscreteActions[2] == 1;
            downArrow = actionBuffers.DiscreteActions[3] == 1;
            leftArrow = actionBuffers.DiscreteActions[4] == 1;
            rightArrow = actionBuffers.DiscreteActions[5] == 1;
        }
        bool[] speedCheck = { walkingUp, walkingDown, walkingLeft, walkingRight };
        bool[] shootCheck = { upArrow, downArrow, leftArrow, rightArrow };
        int amountSpeed = speedCheck.Count(c => c);
        int amountShoot = shootCheck.Count(b => b);
        // Debug.Log("amountSpeed: "+amountSpeed);
        // Debug.Log("amountShoot: "+amountShoot);

        // MOVEMENT Animation
        if (amountShoot == 0)
        {
            switch (walkingUp)
            {
                case true:
                    anim.SetBool("walkingUp", true);
                    break;
                default:
                    anim.SetBool("walkingUp", false);
                    break;
            }
            switch (walkingDown)
            {
                case true:
                    anim.SetBool("walkingDown", true);
                    break;
                default:
                    anim.SetBool("walkingDown", false);
                    break;
            }
            switch (walkingLeft)
            {
                case true:
                    anim.SetBool("walkingLeft", true);
                    break;
                default:
                    anim.SetBool("walkingLeft", false);
                    break;
            }
            switch (walkingRight)
            {
                case true:
                    anim.SetBool("walkingRight", true);
                    break;
                default:
                    anim.SetBool("walkingRight", false);
                    break;
            }
        }

        // MOVEMENT
        float forwardAmount = 0f;
        float turnAmount = 0f;

        if (walkingUp)
        {
            switch (transform.eulerAngles.z)
            {
                case 0:
                    forwardAmount += transform.eulerAngles.z == 0 ? 1f : -1f;
                    break;
                case 180:
                    forwardAmount += transform.eulerAngles.z == 180 ? 1f : -1f;
                    break;
                case 90:
                    turnAmount += transform.eulerAngles.z == 90 ? 1f : -1f;
                    break;
                case 270:
                    turnAmount += transform.eulerAngles.z == 270 ? 1f : -1f;
                    break;
                default:
                    // Edge Case??
                    Debug.Log("Edge Case called: "+"walkingUp");
                    forwardAmount = 1f;
                    break;
            }
        }
        if (walkingDown)
        {
            switch (transform.eulerAngles.z)
            {
                case 0:
                    forwardAmount += transform.eulerAngles.z == 0 ? -1f : 1f;
                    break;
                case 180:
                    forwardAmount += transform.eulerAngles.z == 180 ? -1f : 1f;
                    break;
                case 90:
                    turnAmount += transform.eulerAngles.z == 90 ? -1f : 1f;
                    break;
                case 270:
                    turnAmount += transform.eulerAngles.z == 270 ? -1f : 1f;
                    break;
                default:
                    // Edge Case??
                    Debug.Log("Edge Case called: "+"walkingDown");
                    forwardAmount = -1f;
                    break;
            }
        }
        if (walkingLeft)
        {
            switch (transform.eulerAngles.z)
            {
                case 0:
                    turnAmount += transform.eulerAngles.z == 0 ? -1f : 1f;
                    break;
                case 180:
                    turnAmount += transform.eulerAngles.z == 180 ? -1f : 1f;
                    break;
                case 90:
                    forwardAmount += transform.eulerAngles.z == 90 ? 1f : -1f;
                    break;
                case 270:
                    forwardAmount += transform.eulerAngles.z == 270 ? 1f : -1f;
                    break;
                default:
                    // Edge Case??
                    Debug.Log("Edge Case called: "+"walkingLeft");
                    turnAmount = -1f;
                    break;
            }
        }
        if (walkingRight)
        {
            switch (transform.eulerAngles.z)
            {
                case 0:
                    turnAmount += transform.eulerAngles.z == 0 ? 1f : -1f;
                    break;
                case 180:
                    turnAmount += transform.eulerAngles.z == 180 ? 1f : -1f;
                    break;
                case 90:
                    forwardAmount += transform.eulerAngles.z == 90 ? -1f : 1f;
                    break;
                case 270:
                    forwardAmount += transform.eulerAngles.z == 270 ? -1f : 1f;
                    break;
                default:
                    // Edge Case??
                    Debug.Log("Edge Case called: "+"walkingRight");
                    turnAmount = 1f;
                    break;
            }
        }

        // SHOOTING
        switch (upArrow)
        {
            case true:
                anim.SetBool("walkingUp", true);
                anim.SetBool("walkingDown", false);
                anim.SetBool("walkingLeft", false);
                anim.SetBool("walkingRight", false);
                firingPoint.transform.rotation = Quaternion.Euler(0, 0, 0f);
                break;
        }
        switch (downArrow)
        {
            case true:
                anim.SetBool("walkingUp", false);
                anim.SetBool("walkingDown", true);
                anim.SetBool("walkingLeft", false);
                anim.SetBool("walkingRight", false);
                firingPoint.transform.rotation = Quaternion.Euler(0, 0, 180f);
                break;
        }
        switch (leftArrow)
        {
            case true:
                anim.SetBool("walkingUp", false);
                anim.SetBool("walkingDown", false);
                anim.SetBool("walkingLeft", true);
                anim.SetBool("walkingRight", false);
                firingPoint.transform.rotation = Quaternion.Euler(0, 0, 90f);
                break;
        }
        switch (rightArrow)
        {
            case true:
                anim.SetBool("walkingUp", false);
                anim.SetBool("walkingDown", false);
                anim.SetBool("walkingLeft", false);
                anim.SetBool("walkingRight", true);
                firingPoint.transform.rotation = Quaternion.Euler(0, 0, 270f);
                break;
        }

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
        Vector3 movementForward = agentSettings.moveSpeed * forwardAmount * Time.fixedDeltaTime * transform.up * diagionalSpeed;
        Vector3 movementTurn = agentSettings.moveSpeed * turnAmount * Time.fixedDeltaTime * transform.right * diagionalSpeed;
        Vector3 movement = movementForward + movementTurn;
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