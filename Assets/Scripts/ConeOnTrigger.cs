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
                //Debug.Log("Agent entered cone");
                EnemyAI.inViewCone = true;
            }
        }
        
        void OnTriggerExit2D(Collider2D o)
        {
        
            if (o.gameObject.tag == "agent")
            {
                //Debug.Log("Agent exited cone");
                EnemyAI.inViewCone = false;
            }
        }
    }
}