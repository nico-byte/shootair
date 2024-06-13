using UnityEngine;

namespace ShootAirRLAgent
{
    public class ConeOnTrigger : MonoBehaviour {
    
        EnemyAI EnemyAI;    
    
        void Awake()
        {
            EnemyAI = FindObjectOfType<EnemyAI>();
        }
        
        void OnTriggerEnter2D(Collider2D o)
        {
            if (o.gameObject.tag == "agent")
            {
                EnemyAI.inViewCone = true;
            }
        }
        
        void OnTriggerExit2D(Collider2D o)
        {
        
            if (o.gameObject.tag == "agent")
            {
                EnemyAI.inViewCone = false;
            }
        }
    }
}