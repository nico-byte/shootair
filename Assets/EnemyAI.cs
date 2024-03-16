using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour {
	private Rigidbody2D rBody;
    private RaycastHit2D ray2d;
    private Transform target;
	
	Vector3 direction;
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
    
	public void Awake()
	{
		// Get a reference to the player's transform
        target = GameObject.FindGameObjectWithTag("agent").transform;
 
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
        waypoints = new Transform[9] {
            point1,
            point2,
            point3,
            point4,
            point5,
			point6,
            point7,
            point8,
            point9
        };
	}
	
	public void Start()
	{
		envController = FindObjectOfType<EnvironmentController>();
        rBody = GetComponent<Rigidbody2D>();
	}

	private void Update()
    {
        // If chasing get the position of the player and point towards it
        if (chasing)
        {
            direction = target.position - transform.position;
            rotateEnemy();
        }
 
        // Unless the zombie is waiting then move
        if (!waiting)
        {
            if (!chasing)
            {
                direction = waypoints[currentTarget].position - transform.position;
                rotateEnemy();
            }
            Vector2 rayDir = new Vector2(direction.x, direction.y);
            ray2d = Physics2D.Raycast(rBody.position, new Vector2(direction.x, direction.y), 4f);
            if (ray2d.collider.tag == "obstacle")
                {
                    if (ray2d.distance <= 2f)
                    {
                        rayDir = Vector2.Perpendicular(rayDir);
                        rBody.MovePosition(Vector2.MoveTowards(rBody.position, rBody.position + rayDir, speed * Time.fixedDeltaTime));
                        Debug.DrawRay(rBody.position, rayDir);
                    }
                }
                else
                {
                    rBody.MovePosition(Vector2.MoveTowards(rBody.position, rBody.position + rayDir, speed * Time.fixedDeltaTime));
                    Debug.DrawRay(rBody.position, rayDir);
                }
            // transform.Translate(speed * direction * Time.deltaTime, Space.World);
        }
        
    }

	private void FixedUpdate()
    {
        // Give the values to the FSM (animator)
        distanceFromTarget = Vector3.Distance(waypoints[currentTarget].position, transform.position);
        anim.SetFloat("distanceFromWaypoint", distanceFromTarget);
        anim.SetBool("playerInSight", inViewCone);
 
    }

	public void SetNextPoint()
    {
        // Pick a random waypoint 
        // But make sure it is not the same as the last one
        int lastWaypoint = currentTarget;
        int nextPoint = -1;
 
        do
        {
           nextPoint =  Random.Range(0, waypoints.Length - 1);
           if (nextPoint == 0 && (lastWaypoint == 3 || lastWaypoint == 4 || lastWaypoint == 5 || lastWaypoint == 6))
           {
            SetNextPoint();
           }
        }
        while (nextPoint == currentTarget);
 
        currentTarget = nextPoint;
 
        // Load the direction of the next waypoint
        direction = waypoints[currentTarget].position - transform.position;
        rotateEnemy();
    }
 
    public void Chase()
    {
        // Load the direction of the player
        direction = target.position - transform.position;
        rotateEnemy();
    }
 
    public void StopChasing()
    {
        chasing = false;
    }
 
    private void rotateEnemy()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        direction = direction.normalized;
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
        // ShootairAgent agent = GetComponent<ShootairAgent>();
		if (health <= 0)
		{
			Destroy(gameObject);
			envController.ResolveEvent(Event.killedTarget);
		}
	}
}