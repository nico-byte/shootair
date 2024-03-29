using System.Collections;
using UnityEngine;

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
            // EnemyAI.inViewCone = true;
            EnemyAI[] EnemyArray = FindObjectsOfType<EnemyAI>();
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
            // EnemyAI.inViewCone = false;
            EnemyAI[] EnemyArray = FindObjectsOfType<EnemyAI>();
            foreach (EnemyAI enemy in EnemyArray)
            {
                enemy.inViewCone = false;
            }
        }
    }
}