using UnityEngine;

namespace ShootAirRLAgent
{
	public class Bullet : MonoBehaviour {
		EnvironmentController envController;
	
		[SerializeField]
		private float speed = 20f;
		[SerializeField]
		private int damage = 40;
		[SerializeField]
		private Rigidbody2D rb;
	
	    [SerializeField]
		private float lifeTime = 3f;
	
		// Use this for initialization
		void Start () 
		{
			envController = FindObjectOfType<EnvironmentController>();
			rb = GetComponent<Rigidbody2D>();
	        Destroy(this.gameObject, lifeTime);
		}
	
	    private void FixedUpdate()
	    {
	        rb.velocity = transform.up * speed;
	    }
	
		private void OnTriggerEnter2D (Collider2D other)
	    {
			if (other.gameObject.CompareTag("target") || other.gameObject.CompareTag("wall") || other.gameObject.CompareTag("obstacle"))
			{
				if (!other.gameObject.CompareTag("target"))
				{
					envController.ResolveEvent(Event.missedShot);
				}
				Destroy(gameObject);
			}
			EnemyAI enemy = other.GetComponent<EnemyAI>();
			if (enemy != null)
			{
				enemy.TakeDamage(damage);
				Debug.Log("Hit!");
				envController.ResolveEvent(Event.hitOnTarget);
			}
	    }
	}
}