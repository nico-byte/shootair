using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.MLAgents;
using UnityEngine;

public enum Event
{
    hitOnTarget = 0,
    collisionWithTarget = 1,
    killedTarget = 2,
    killedAllTargets = 3
}

public class EnvironmentController : MonoBehaviour
{
    EnvironmentSettings environmentSettings;

    public ShootairAgent shootairAgent;
    public Enemy enemy;
    public Bullet bullet;

    public GameObject enemyPrefab;

    public List<ShootairAgent> AgentsList = new List<ShootairAgent>();
    public List<GameObject> EnemyList = new List<GameObject>();
    
    private int resetTimer;
    public int MaxEnvironmentSteps;
    
    // Start is called before the first frame update
    void Start()
    {
        environmentSettings = FindObjectOfType<EnvironmentSettings>();

        ResetScene();
    }

    public void ResolveEvent(Event triggerEvent)
    {
        switch (triggerEvent)
        {
            case Event.hitOnTarget:
                
                // apply reward to shootair agent
                shootairAgent.AddReward(1e6f);

                break;

            case Event.collisionWithTarget:
                // agent loses
                shootairAgent.AddReward(-1f);

                // end episode
                shootairAgent.EndEpisode();
                ResetScene();
                break;

            case Event.killedTarget:
                // add reward for killing target
                shootairAgent.AddReward(-1e2f);

                break;

            case Event.killedAllTargets:
                // agent wins
                shootairAgent.AddReward(1f);

                // end episode
                shootairAgent.EndEpisode();
                ResetScene();
                break;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        resetTimer += 1;
        if (resetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            shootairAgent.EpisodeInterrupted();
            ResetScene();
        }

        EnemyList.RemoveAll( x => !x);

        if (EnemyList.Count == 0)
        {
            ResolveEvent(Event.killedAllTargets);
        }
    }

    public void ResetScene()
    {
        foreach (var agent in AgentsList)
        {
            // randomise starting positions and rotations
            var randomPosX = Random.Range(-2f, 2f);
            var randomPosZ = Random.Range(-2f, 2f);
            var randomPosY = Random.Range(0.5f, 3.75f); // depends on jump height
            var randomRot = Random.Range(-45f, 45f);

            agent.transform.localPosition = new UnityEngine.Vector3(randomPosX, randomPosY, randomPosZ);
            agent.transform.eulerAngles = new UnityEngine.Vector3(0, randomRot, 0);

            agent.GetComponent<Rigidbody>().velocity = default;
        }

        ResetEnemies();
    }

    void ResetEnemies()
    {
        // kill previous instances of enemies
        Object[] allObjects = FindObjectsOfType(typeof(GameObject));
        foreach(GameObject obj in allObjects) {
            if(obj.transform.name == "Enemy(Clone)"){
                Destroy(obj);
            }
        }

        // Move enemies in spawn area
        for (int i = 0; i <= environmentSettings.numEnemies; i++)
        {
            GameObject newGO = Instantiate(enemyPrefab, new UnityEngine.Vector3(Random.value * 12 + 4, Random.value * 8 - 4, 0f), UnityEngine.Quaternion.Euler(0f, 0f, Random.Range(0.0f, 360.0f)));
            EnemyList.Add(newGO);
        }

        // Spawn enemies in spawn area
        for (int i = 0; i <= environmentSettings.numEnemies; i++)
        {
            GameObject newGO = Instantiate(enemyPrefab, new UnityEngine.Vector3(Random.value * - 12 - 4, Random.value * 8 - 4, 0f), UnityEngine.Quaternion.Euler(0f, 0f, Random.Range(0.0f, 360.0f)));
            EnemyList.Add(newGO);
        }
    }
}
