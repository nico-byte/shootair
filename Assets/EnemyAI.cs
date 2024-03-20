using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.MLAgents;
using Unity.Sentis.Layers;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour {
	private Rigidbody2D rBody;
    private RaycastHit2D ray2d;
    private Transform target;
	
	UnityEngine.Vector3 direction;
	private int currentTarget;
    private Transform[] currentPath = null;
	private Transform[] mainPath = null;
    private Transform[] secPath1 = null;
    private Transform[] secPath2 = null;
    private Transform[] secPath3 = null;
    private Transform[] secPath4 = null;
    private List<Transform[]> paths = null;

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
		// Get a reference to the agent's transform
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
        Transform point10 = GameObject.Find("p10").transform;
        Transform point11 = GameObject.Find("p11").transform;
        Transform point12 = GameObject.Find("p12").transform;
        Transform point13 = GameObject.Find("p13").transform;
        Transform point14 = GameObject.Find("p14").transform;
		Transform point15 = GameObject.Find("p15").transform;
        Transform point22 = GameObject.Find("p22").transform;
        Transform point16 = GameObject.Find("p16").transform;
        mainPath = new Transform[16] {
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
            point16
        };

        Transform point17 = GameObject.Find("p17").transform;
        Transform point19 = GameObject.Find("p19").transform;
        secPath1 = new Transform[2] {
            point17,
            point19
        };

        Transform point20 = GameObject.Find("p20").transform;
        Transform point18 = GameObject.Find("p18").transform;
        secPath2 = new Transform[2] {
            point20,
            point18
        };

        Transform point21 = GameObject.Find("p21").transform;
        secPath3 = new Transform[3] {
            point20,
            point18,
            point21
        };

        secPath3 = new Transform[3] {
            point20,
            point18,
            point21
        };

        paths = new List<Transform[]> {
            mainPath,
            secPath1,
            secPath2,
            secPath3,
            secPath4
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
            direction = target.position - transform.position;
            rotateEnemy();
        }
 
        // Unless the enemy is waiting then move
        if (!waiting)
        {
            if (!chasing)
            {
                direction = mainPath[currentTarget].position - transform.position;
                rotateEnemy();
            }
            UnityEngine.Vector2 rayDir = new UnityEngine.Vector2(direction.x, direction.y);
            ray2d = Physics2D.Raycast(rBody.position, new UnityEngine.Vector2(direction.x, direction.y), 4f);
            if (ray2d.collider.tag == "obstacle")
                {
                    if (ray2d.distance <= 2f)
                    {
                        rayDir = UnityEngine.Vector2.Perpendicular(rayDir);
                        rBody.MovePosition(UnityEngine.Vector2.MoveTowards(rBody.position, rBody.position + rayDir, speed * Time.fixedDeltaTime));
                        Debug.DrawRay(rBody.position, rayDir);
                    }
                }
                else
                {
                    rBody.MovePosition(UnityEngine.Vector2.MoveTowards(rBody.position, rBody.position + rayDir, speed * Time.fixedDeltaTime));
                    Debug.DrawRay(rBody.position, rayDir);
                }
        }
        
    }

	private void FixedUpdate()
    {
        // Give the values to the FSM (animator)
        distanceFromTarget = UnityEngine.Vector3.Distance(mainPath[currentTarget].position, transform.position);
        anim.SetFloat("distanceFromWaypoint", distanceFromTarget);
        anim.SetBool("playerInSight", inViewCone);
 
    }

	public void SetNextPoint()
    {
        // Pick a random waypoint 
        // But make sure it is not the same as the last one
        if (currentPath == null)
        {
            currentPath = mainPath;
            int idxMinDistance = 100;
            float minDistance = 100;
            for (int i = 0; i<=mainPath.Length-1; i++)
            {
                if (UnityEngine.Vector2.Distance(transform.position, mainPath[i].position) < minDistance)
                {
                    idxMinDistance = i;
                }
            }
            currentTarget = idxMinDistance;

            direction = mainPath[idxMinDistance].position - transform.position;
            rotateEnemy();
        }

        int nextPoint = -1;

        bool goMainPath = currentTarget != 11 || currentTarget != 13 || currentTarget != 14 || currentTarget != 16 || currentTarget != 22 || currentTarget != 5 || currentTarget != 3 || currentTarget != 6 || currentTarget != 8;
 
        bool idxIn = currentTarget <= currentPath.Length-1;
        do
        {   
            nextPoint = idxIn ? currentTarget+1 : 0;
            SetNextPoint();
        }
        while (nextPoint == currentTarget);
 
        currentTarget = nextPoint;
 
        // Load the direction of the next waypoint
        direction = currentPath[currentTarget].position - transform.position;
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
        transform.rotation = UnityEngine.Quaternion.Euler(new UnityEngine.Vector3(0, 0, angle - 90));
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
		if (health <= 0)
		{
			Destroy(gameObject);
			envController.ResolveEvent(Event.killedTarget);
		}
	}
}