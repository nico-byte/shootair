using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public ShootairAgent agent;
	EnvironmentController envController;

	public int health = 100;
    
	public void Start()
	{
		envController = FindObjectOfType<EnvironmentController>();;
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