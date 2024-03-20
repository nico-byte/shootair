using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.Sentis.Layers;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour {
	EnvironmentController envController;

	public float speed = 20f;
	public int damage = 40;
	public Rigidbody2D rb;

    public float lifeTime = 3f;

	// Use this for initialization
	void Start () 
	{
		envController = FindObjectOfType<EnvironmentController>();;
		rb = GetComponent<Rigidbody2D>();
        Destroy(this.gameObject, lifeTime);
	}

    private void FixedUpdate()
    {
        rb.velocity = transform.up * speed;
    }

	private void OnTriggerEnter2D (Collider2D other)
    {
		if (other.gameObject.CompareTag("target") || other.gameObject.CompareTag("wall"))
		{
			Destroy(gameObject);
		}
		EnemyAI enemy = other.GetComponent<EnemyAI>();
		if (enemy != null)
		{
			enemy.TakeDamage(damage);
			envController.ResolveEvent(Event.hitOnTarget);
		}
    }
}