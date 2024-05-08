using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public int health = 100;
    // private Agent agent;
    public void TakeDamage (int damage)
	{
		health -= damage;
        // ShootairAgent agent = GetComponent<ShootairAgent>();
		if (health <= 0)
		{
			Destroy(gameObject);
            // Debug.Log("Killed object!");
		}
	}

}