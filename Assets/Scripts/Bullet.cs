using UnityEngine;

namespace ShootAirRLAgent
{
	public class Bullet : MonoBehaviour
	{
		EnvironmentController envController;
		[SerializeField]
		private Rigidbody2D rb;

		[SerializeField]
		private float speed = 0f;
		[SerializeField]
		private int damage = 0;
		[SerializeField]
		private float lifeTime = 0f;

		// Use this for initialization
		void Start()
		{
			envController = FindObjectOfType<EnvironmentController>();
			rb = GetComponent<Rigidbody2D>();
			Destroy(this.gameObject, lifeTime);
		}

		private void FixedUpdate()
		{
			if(speed != 0f) {
				rb.velocity = transform.up * speed;
			}
		}

		public void bulletSettings(float speed, int damage, float lifetime)
		{
			this.speed = speed;
			this.damage = damage;
			this.lifeTime = lifetime;
		}

		private void OnTriggerEnter2D(Collider2D other)
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