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

    public GameObject standardEnemyPrefab;
    public GameObject difficultEnemyPrefab;
    public GameObject fastEnemyPrefab;

    // public List<ShootairAgent> AgentsList = new List<ShootairAgent>();
    public List<GameObject> EnemyList = new List<GameObject>();
    
    private int resetTimer;
    public int MaxEnvironmentSteps;
    private int currentWave = 0;
    
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

                currentWave = 0;

                // end episode
                shootairAgent.EndEpisode();
                ResetScene();
                break;

            case Event.killedTarget:
                // add reward for killing target
                shootairAgent.AddReward(1e3f);

                break;

            case Event.killedAllTargets:
                if (currentWave >= environmentSettings.waves.Count-1)
                {
                    currentWave = 0;
                    shootairAgent.AddReward(.5f);
                    shootairAgent.EndEpisode();
                    ResetScene();
                    break;
                }
                
                // agent wins
                shootairAgent.AddReward(2e2f);

                // end episode
                shootairAgent.EpisodeInterrupted();

                currentWave++;
                ResetEnemies();
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
        /*
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
        */

        // randomise starting positions and rotations
        var randomPosX = Random.Range(-2f, 2f);
        var randomPosY = Random.Range(0.5f, 3.75f); // depends on jump height
        var randomRot = Random.Range(-45f, 45f);

        shootairAgent.transform.localPosition = new UnityEngine.Vector3(randomPosX, randomPosY, 0);
        shootairAgent.transform.eulerAngles = new UnityEngine.Vector3(0, 0, randomRot);

        shootairAgent.GetComponent<Rigidbody2D>().velocity = default;

        ResetEnemies();
    }

    void ResetEnemies()
    {
        // kill previous instances of enemies
        Object[] allObjects = FindObjectsOfType(typeof(GameObject));
        foreach(GameObject obj in allObjects) {
            if(obj.transform.name == "StandardEnemy(Clone)" || obj.transform.name == "DifficultEnemy(Clone)" || obj.transform.name == "FastEnemy(Clone)"){
                Destroy(obj);
            }
        }

        List<int> enemyCount = environmentSettings.waves[currentWave];

        if (enemyCount[0] > 0)
        {
            spawn(standardEnemyPrefab, enemyCount[0], 16, 4);
        }

        if (enemyCount[1] > 0)
        {
            spawn(fastEnemyPrefab, enemyCount[1], -16, 6);
        }

        if (enemyCount[2] > 0)
        {
            spawn(difficultEnemyPrefab, enemyCount[2], -16, 2);
        }
    }

    void spawn(GameObject prefab, int quant, int xOffset, int yOffset)
    {
        // Spawn enemies in spawn area
        for (int i = 1; i <= quant; i++)
        {
            GameObject newGO = Instantiate(prefab, new UnityEngine.Vector3(Random.value * xOffset, Random.value * yOffset, 0f), UnityEngine.Quaternion.Euler(0f, 0f, Random.Range(0.0f, 360.0f)));
            EnemyList.Add(newGO);
        }
    }
}
