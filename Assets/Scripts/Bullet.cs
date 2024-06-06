using UnityEngine;

namespace ShootAirRLAgent
{
	public class Bullet : MonoBehaviour
	{
		//OBJECTS
		EnvironmentController envController;
		SoundEffectPlayer soundHandler;

		[SerializeField]
		private Rigidbody2D rb;

		[SerializeField]
		private float speed = 0f;
		[SerializeField]
		private int damage = 0;
		[SerializeField]
		private float lifeTime = 0f;

		void Start()
		{
			envController = FindObjectOfType<EnvironmentController>();
			soundHandler = FindObjectOfType<SoundEffectPlayer>();
			rb = GetComponent<Rigidbody2D>();
			Destroy(this.gameObject, lifeTime); // Destroy Bullet after Lifetime
		}

		private void FixedUpdate()
		{
			// Bullet Speed Handling
			if (speed != 0f)
			{
				rb.velocity = transform.up * speed;
			}
		}

		public void bulletSettings(float speed, int damage, float lifetime)
		{
			// Change Bullet Settings
			this.speed = speed;
			this.damage = damage;
			this.lifeTime = lifetime;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			EnemyAI enemy = other.GetComponent<EnemyAI>();
			if (other.gameObject.CompareTag("wall")) // Bullet hit Wall
			{
				Destroy(gameObject);
			}
			else if (enemy != null) // Bullet hit Enemy
			{
				Destroy(gameObject);
				enemy.TakeDamage(damage);
				envController.ResolveEvent(Event.hitOnTarget);
			}
		}
	}
}