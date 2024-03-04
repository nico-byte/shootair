using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public ShootairAgent agent;
	private Transform target;
	public float speed;
	EnvironmentController envController;

	public int health;
    
	public void Start()
	{
		envController = FindObjectOfType<EnvironmentController>();
		target = GameObject.FindGameObjectWithTag("agent").GetComponent<Transform>();
	}

	void Update()
	{
		Move();
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

	private void Move()
	{
		transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
	}

}