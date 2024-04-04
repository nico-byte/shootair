using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.MLAgents;
using Unity.Sentis.Layers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {
	private Rigidbody2D rBody;
    private RaycastHit2D ray2d;
    private Transform target;

    NavMeshAgent enemy;
	
	UnityEngine.Vector3 direction;
	private int currentTarget;
	private Transform[] waypoints = null;

	public float speed;

	private Animator anim;
	bool chasing = false;
	bool waiting = false;
	private float distanceFromTarget;
	public bool inViewCone;

	EnvironmentController envController;
	public int health;

    private UnityEngine.Vector2 originalPosition;

    private UnityEngine.Vector2 previousPosition;
    private int stuckCounter;
    public UnityEngine.Vector2 trackVelocity;
    
	public void Awake()
	{
		// Get a reference to the agent's transform
        target = GameObject.FindGameObjectWithTag("agent").transform;

        enemy = GetComponent<NavMeshAgent>();
        enemy.updateRotation = false;
        enemy.updateUpAxis = false;
 
        // Get a reference to the FSM (animator)
        anim = gameObject.GetComponent<Animator>();
 
        // Add all our waypoints into the waypoints array
        Transform point1 = GameObject.Find("p1").transform;
        Transform point2 = GameObject.Find("p2").transform;
        Transform point3 = GameObject.Find("p3").transform;
        Transform point4 = GameObject.Find("p4").transform;
        Transform point5 = GameObject.Find("p5").transform;
		Transform point6 = GameObject.Find("p6").transform;
        Transform point7 = GameObject.Find("p7").transform;
        Transform point8 = GameObject.Find("p8").transform;
        Transform point9 = GameObject.Find("p9").transform;
        Transform point10 = GameObject.Find("p10").transform;
        Transform point11 = GameObject.Find("p11").transform;
        Transform point12 = GameObject.Find("p12").transform;
        Transform point13 = GameObject.Find("p13").transform;
        Transform point14 = GameObject.Find("p14").transform;
		Transform point15 = GameObject.Find("p15").transform;
        Transform point16 = GameObject.Find("p16").transform;
        Transform point17 = GameObject.Find("p17").transform;
        Transform point18 = GameObject.Find("p18").transform;
        Transform point19 = GameObject.Find("p19").transform;
        Transform point20 = GameObject.Find("p20").transform;
        Transform point21 = GameObject.Find("p21").transform;
        waypoints = new Transform[21] {
            point1,
            point2,
            point3,
            point4,
            point5,
			point6,
            point7,
            point8,
            point9,
            point10,
            point11,
            point12,
            point13,
            point14,
            point15,
            point16,
            point17,
            point18,
            point19,
            point20,
            point21
        };
	}
	
	public void Start()
	{
		envController = FindObjectOfType<EnvironmentController>();
        rBody = GetComponent<Rigidbody2D>();
	}

	private void Update()
    {   
        // If chasing get the position of the agent and point towards it
        if (chasing)
        {
            Chase();
            rotateEnemy();
        }
 
        // Unless the enemy is waiting then move
        if (!waiting)
        {
            if (!chasing)
            {
                NavMeshPath path = new NavMeshPath();
                enemy.CalculatePath(waypoints[currentTarget].position, path);
                enemy.SetPath(path);
                // enemy.SetDestination(new UnityEngine.Vector3(waypoints[currentTarget].position.x, waypoints[currentTarget].position.y, transform.position.z));
                direction = new UnityEngine.Vector2(transform.position.x, transform.position.y) - originalPosition;
                rotateEnemy();
                originalPosition = transform.position;
            }
        }
        
    }

	private void FixedUpdate()
    {
        // Give the values to the FSM (animator)
        distanceFromTarget = UnityEngine.Vector3.Distance(waypoints[currentTarget].position, transform.position);
        anim.SetFloat("distanceFromWaypoint", distanceFromTarget);
        anim.SetBool("playerInSight", inViewCone);
        // Debug.Log(inViewCone);

        trackVelocity = (rBody.position - previousPosition) * 50;
        
        distanceFromTarget = UnityEngine.Vector3.Distance(waypoints[currentTarget].position, transform.position);
        // Check if the enemy is stuck
        if (UnityEngine.Vector3.Distance(transform.position, previousPosition) < 1f)
        {
            stuckCounter++;
        }
        else
        {
            stuckCounter = 0;
        }

        // Update previousPosition for the next frame
        previousPosition = rBody.position;

        if (stuckCounter > 100)
        {
            SetNextPoint();
            stuckCounter = 0;
        }
    }

	public void SetNextPoint()
    {
        // Pick a random waypoint 
        // But make sure it is not the same as the last one
        int nextPoint = -1;
 
        do
        {
            nextPoint =  Random.Range(0, waypoints.Length - 1);
        }
        while (nextPoint == currentTarget);
 
        currentTarget = nextPoint;
        
        direction = waypoints[currentTarget].position - transform.position;
        rotateEnemy();
    }
 
    public void Chase()
    {
        // Load the direction of the player
        NavMeshPath path = new NavMeshPath();
        enemy.CalculatePath(target.position, path);
        enemy.SetPath(path);
        // enemy.SetDestination(new UnityEngine.Vector3(target.position.x, target.position.y, transform.position.z));
        direction = target.position - transform.position;
        rotateEnemy();
    }
 
    public void StopChasing()
    {
        chasing = false;
        // enemy.ResetPath();
    }

    private void rotateEnemy()
    {   
        direction = direction.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        UnityEngine.Quaternion lookRotation = UnityEngine.Quaternion.Euler(new UnityEngine.Vector3(0, 0, angle - 90));
        transform.rotation = UnityEngine.Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
        
    }
 
    public void StartChasing()
    {
        chasing = true;
    }
 
 
    public void ToggleWaiting()
    {
        waiting  = !waiting;
    }
	
	public void TakeDamage (int damage)
	{
		health -= damage;
		if (health <= 0)
		{
			Destroy(gameObject);
			envController.ResolveEvent(Event.killedTarget);
		}
	}
}