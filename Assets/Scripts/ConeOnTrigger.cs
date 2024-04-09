using UnityEngine;

namespace ShootAirRLAgent
{
    public class ConeOnTrigger : MonoBehaviour {
    
        EnemyAI EnemyAI;
        EnemyAI[] EnemyArray;
    
    
        void Awake()
        {
            EnemyAI = FindObjectOfType<EnemyAI>();
        }
        
        void FixedUpdate()
        {
            EnemyArray = FindObjectsOfType<EnemyAI>();
        }
        
        void OnTriggerEnter2D(Collider2D o)
        {
            if (o.gameObject.tag == "agent")
            {
                EnemyAI.inViewCone = true;
                foreach (EnemyAI enemy in EnemyArray)
                {
                    enemy.inViewCone = true;
                }
            }
        }
        
        void OnTriggerExit2D(Collider2D o)
        {
        
            if (o.gameObject.tag == "agent")
            {
                EnemyAI.inViewCone = false;
                foreach (EnemyAI enemy in EnemyArray)
                {
                    enemy.inViewCone = false;
                }
            }
        }
    }
}