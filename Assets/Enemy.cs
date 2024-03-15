using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.VisualScripting;
using UnityEngine;

public enum AIState
{
	idle = 0,
	walk = 1
}

public class Enemy : MonoBehaviour {
	public Vector2 idleRange;
	public Vector2 walkRange;
	
	public ShootairAgent agent;
	private Transform target;
	private Rigidbody2D rBody;
	public float speed;

	EnvironmentController envController;
	public int health;
    
	public void Start()
	{
		rBody = GetComponent<Rigidbody2D>();
		envController = FindObjectOfType<EnvironmentController>();
		target = GameObject.FindGameObjectWithTag("agent").GetComponent<Transform>();
	}

	void Update()
	{
		DecideMovement();
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

	private void MoveToTarget()
	{
		Vector2 direction = (target.position - transform.position).normalized;
     	var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; 
     	var offset = 90f;
     	transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
		transform.position = Vector2.MoveTowards(transform.position, target.position, speed/6 * Time.deltaTime);
	}

	void DecideMovement()
	{
		var hit = Physics2D.Linecast(transform.position + new Vector3(0.0f, 0.6f, 0), target.position);
		Debug.DrawRay(transform.position, target.position, Color.green);
		Debug.Log(hit.collider.tag);
		
		if (hit.collider.CompareTag("agent"))
        {
            MoveToTarget();
        }
		else
		{
			RandomMovement();
		}
	}

	private void RandomMovement()
	{
		Vector3 dir = new Vector3(Random.value, Random.value, 0);
		dir = transform.position - dir;
		float Duration = Random.Range(walkRange.x, walkRange.y);
		
		var lookRotation = Quaternion.LookRotation(target.position - transform.position);
		lookRotation.x = lookRotation.y = 0;
		transform.rotation = lookRotation;
			
		while(Duration > 0f)
		{
			Duration -= Time.deltaTime;
			transform.position = Vector2.MoveTowards(transform.position, dir, speed/10 * Time.deltaTime);
		}
	}
}