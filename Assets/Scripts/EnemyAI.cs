using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace ShootAirRLAgent
{
    public class EnemyAI : MonoBehaviour
    {
        private Rigidbody2D rBody;
        private Transform target;

        NavMeshAgent enemy;

        Vector3 direction;
        private Vector2 currentTarget;
        private Animator anim;
        bool chasing = false;
        bool waiting = false;
        public bool inViewCone { get; set; }

        EnvironmentController envController;
        public int health { get; set; }

        [SerializeField]
        private int enemyType;

        private Vector2 originalPosition;

        private Vector2 previousPosition;
        private int stuckCounter;
        public Vector2 trackVelocity { get; set; }

        public void Awake()
        {
            // Get a reference to the agent's transform
            target = GameObject.FindGameObjectWithTag("agent").transform;

            enemy = GetComponent<NavMeshAgent>();
            enemy.updateRotation = false;
            enemy.updateUpAxis = false;

            // Get a reference to the FSM (animator)
            anim = gameObject.GetComponent<Animator>();
        }

        public void Start()
        {
            envController = FindObjectOfType<EnvironmentController>();
            rBody = GetComponent<Rigidbody2D>();

            if (enemyType == 1)
            {
                health = 150;
            }
            else if (enemyType == 2)
            {
                health = 30;
            }
            else
            {
                health = 100;
            }
        }

        private void UpdateMovement()
        {
            // If chasing get the position of the agent and point towards it
            if (chasing)
            {
                Chase();
                rotateEnemy();
            }

            // Unless the enemy is waiting then move
            if (!waiting && !chasing)
            {
                NavMeshPath path = new NavMeshPath();
                enemy.CalculatePath(currentTarget, path);
                enemy.SetPath(path);
                direction = new UnityEngine.Vector2(transform.position.x, transform.position.y) - originalPosition;
                rotateEnemy();
                originalPosition = transform.position;
            }

        }

        private void FixedUpdate()
        {
            UpdateMovement();
            // Give the values to the FSM (animator)
            float distanceFromTarget = Vector2.Distance(currentTarget, transform.position);
            anim.SetFloat("distanceFromWaypoint", distanceFromTarget);
            anim.SetBool("playerInSight", inViewCone);

            trackVelocity = (rBody.position - previousPosition) * 50;

            // Check if the enemy is stuck
            if (Vector3.Distance(transform.position, previousPosition) < 1f)
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
            Vector2 nextPoint;
            Vector2 targetPosition = new Vector2(target.transform.position.x, target.transform.position.y);

            do
            {
                nextPoint = RandomPointInAnnulus(targetPosition, 2.5f, 5f);
            }
            while (nextPoint == currentTarget);

            currentTarget = nextPoint;

            direction = currentTarget - new Vector2(transform.position.x, transform.position.y);
            rotateEnemy();
        }

        private Vector2 RandomPointInAnnulus(Vector2 center, float minRadius, float maxRadius)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized; // There are more efficient ways, but well
            float minRadius2 = minRadius * minRadius;
            float maxRadius2 = maxRadius * maxRadius;
            float randomDistance = Mathf.Sqrt(Random.value * (maxRadius2 - minRadius2) + minRadius2);
            Vector2 point = center + randomDirection * randomDistance;
            
            return point;
        }
    
        public void Chase()
        {
            // Load the direction of the player
            NavMeshPath path = new NavMeshPath();
            enemy.CalculatePath(target.position, path);
            enemy.SetPath(path);
            direction = target.position - transform.position;
            rotateEnemy();
        }
    
        public void StopChasing()
        {
            chasing = false;
        }

        private void rotateEnemy()
        {   
            direction = direction.normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion lookRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);

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
}