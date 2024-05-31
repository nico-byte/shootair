using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace ShootAirRLAgent
{
    public class EnemyAI : MonoBehaviour
    {
        private Rigidbody2D rBody;
        private Transform target;
        private NavMeshAgent enemy;
        private Vector3 direction;
        private Vector2 currentTarget;
        private Animator anim;
        private bool chasing = false;
        private bool waiting = false;
        public bool inViewCone { get; set; }
        private EnvironmentController envController;
        public int health { get; set; }

        [SerializeField]
        private int enemyType;

        private Vector2 originalPosition;
        private Vector2 previousPosition;
        private int stuckCounter;
        public Vector2 trackVelocity { get; set; }

        public void Awake()
        {
            target = GameObject.FindGameObjectWithTag("agent").transform;
            enemy = GetComponent<NavMeshAgent>();
            enemy.updateRotation = false;
            enemy.updateUpAxis = false;
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
            else if (enemyType == 3)
            {
                health = 100;
            }
            else if (enemyType == 4)
            {
                health = 200;
            }
            else 
            {
                health = 250;
            }

            // optimizing NavMesh settings
            enemy.speed = 3.5f; // agent speed
            enemy.acceleration = 8f; // acceleration speed 
            enemy.angularSpeed = 120f; // angle speed
            enemy.stoppingDistance = 0.5f; // distance from target
        }

        private void Update()
        {
            if (chasing)
            {
                Chase();
            }
            else if (!waiting)
            {
                MoveToCurrentTarget();
            }
        }

        private void FixedUpdate()
        {
            float distanceFromTarget = Vector2.Distance(currentTarget, transform.position);
            anim.SetFloat("distanceFromWaypoint", distanceFromTarget);
            anim.SetBool("playerInSight", inViewCone);

            trackVelocity = (rBody.position - previousPosition) * 50;

            if (Vector3.Distance(transform.position, previousPosition) < 1f)
            {
                stuckCounter++;
            }
            else
            {
                stuckCounter = 0;
            }

            previousPosition = rBody.position;

            if (stuckCounter > 100)
            {
                SetNextPoint();
                stuckCounter = 0;
            }
        }

        public void SetNextPoint()
        {
            Vector2 nextPoint;
            Vector2 targetPosition = new Vector2(target.transform.position.x, target.transform.position.y);

            do
            {
                nextPoint = RandomPointInAnnulus(targetPosition, 2.5f, 5f);
            }
            while (nextPoint == currentTarget);

            currentTarget = nextPoint;
            MoveToCurrentTarget();
        }

        private Vector2 RandomPointInAnnulus(Vector2 center, float minRadius, float maxRadius)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float minRadius2 = minRadius * minRadius;
            float maxRadius2 = maxRadius * maxRadius;
            float randomDistance = Mathf.Sqrt(Random.value * (maxRadius2 - minRadius2) + minRadius2);
            Vector2 point = center + randomDirection * randomDistance;

            return point;
        }

        public void Chase()
        {
            enemy.SetDestination(target.position);
            UpdateAnimation(enemy.velocity.normalized);
        }

        public void StopChasing()
        {
            chasing = false;
        }

        public void MoveToCurrentTarget()
        {
            enemy.SetDestination(currentTarget);
            UpdateAnimation(enemy.velocity.normalized);
        }

        void UpdateAnimation(Vector3 direction)
        {
            float horizontal = direction.x;
            float vertical = direction.y;

            // initialising parameters for movement
            float xMove = 0;
            float yMove = 0;

            // define main direction
            if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
            {
                if (horizontal > 0)
                {
                    // Enemy movement "right"
                    xMove = 1.1f;
                    yMove = 0;
                }
                else 
                {
                    // Enemy movement "left"
                    xMove = -1.1f;
                    yMove = 0;
                }
                   
            }
            else
            {
                if (vertical > 0)
                {
                    // Enemy movement "up"
                    xMove = 0;
                    yMove = 1.1f;
                }
                else
                {
                    // Enemy movement "down"
                    xMove = 0;
                    yMove = -1.1f;
                }
            }

            // setting animation parameters
            anim.SetFloat("xMove", xMove);
            anim.SetFloat("yMove", yMove);

            // Static enemy angle
            float angle = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, 0); // enemy stays in right angle
        }

        public void StartChasing()
        {
            chasing = true;
        }

        public void ToggleWaiting()
        {
            waiting = !waiting;
        }

        public void TakeDamage(int damage)
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
