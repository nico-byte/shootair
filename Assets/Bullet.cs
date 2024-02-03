using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.Sentis.Layers;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public float speed = 20f;
	public int damage = 40;
	public Rigidbody2D rb;

	// public GameObject impactEffect;
	public static bool hitTarget = false;
	public static bool hitWall = false;
	public static bool kill = false;
    public float lifeTime = 3f;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
        Destroy(this.gameObject, lifeTime);
	}

    private void FixedUpdate()
    {
        rb.velocity = transform.up * speed;
    }

	private void OnTriggerEnter2D (Collider2D other)
    {
        ShootairAgent agent = other.GetComponent<ShootairAgent>();
		if (other.CompareTag("target"))
		{
			hitTarget = true;
			// Debug.Log("Hit Target!");
		}
		if (other.CompareTag("wall"))
		{
			hitWall = true;
			// Debug.Log("Hit Wall!");
		}
		if (other.gameObject.CompareTag("target") || other.gameObject.CompareTag("wall"))
		{
			Destroy(gameObject);
		}
		Enemy enemy = other.GetComponent<Enemy>();
		if (enemy != null)
		{
			enemy.TakeDamage(damage);
		}
    }
}