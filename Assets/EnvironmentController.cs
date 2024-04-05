using System.Collections.Generic;
using UnityEngine;
namespace ShootAirRLAgent
{
    public enum Event
    {
        hitOnTarget = 0,
        collisionWithTarget = 1,
        killedTarget = 2,
        killedAllTargets = 3,
        hitWall = 4,
        missedShot = 5
    }

    public class EnvironmentController : MonoBehaviour
    {
        EnvironmentSettings environmentSettings;
        AgentSettings agentSettings;

        [SerializeField]
        private ShootairAgent shootairAgent;

        [SerializeField]
        private GameObject trainingAreaPrefab;
        [SerializeField]
        private GameObject standardEnemyPrefab;
        [SerializeField]
        private GameObject difficultEnemyPrefab;
        [SerializeField]
        private GameObject fastEnemyPrefab;
        private GameObject agent;
        private GameObject area;

        [SerializeField]
        private Transform environment;

        public List<GameObject> EnemyList { get; set; } = new List<GameObject>();

        private int resetTimer;
        [SerializeField]
        private int MaxEnvironmentSteps;
        private int currentWave = 0;
        private int streak;

        // Start is called before the first frame update
        void Start()
        {
            environmentSettings = FindObjectOfType<EnvironmentSettings>();

            area = Instantiate(trainingAreaPrefab, environment.position, environment.rotation);
            agent = GameObject.FindGameObjectWithTag("agent");

            agent.transform.parent = area.transform;

            ResetScene();
        }

        public void ResolveEvent(Event triggerEvent)
        {
            switch (triggerEvent)
            {
                case Event.hitOnTarget:

                    // apply reward to shootair agent
                    shootairAgent.AddReward(1e-5f + streak * 1e-5f);
                    streak += 1;

                    break;

                case Event.collisionWithTarget:
                    // agent loses
                    shootairAgent.SetReward(-.9f);

                    currentWave = 0;

                    // end episode
                    shootairAgent.EndEpisode();
                    ResetScene();
                    break;

                case Event.killedTarget:
                    // add reward for killing target
                    shootairAgent.AddReward(6e-3f);

                    break;

                case Event.killedAllTargets:
                    if (currentWave >= environmentSettings.waves.Count-1)
                    {
                        currentWave = 0;
                        shootairAgent.EndEpisode();
                        ResetScene();
                        break;
                    }

                    // agent wins
                    shootairAgent.AddReward(.6f / environmentSettings.waves.Count);

                    // end episode

                    currentWave++;
                    ResetEnemies();
                    break;

                case Event.missedShot:

                    // apply reward to shootair agent
                    shootairAgent.AddReward(-1e-5f);
                    streak = 0;

                    break;
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            resetTimer += 1;
            if (resetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
            {
                shootairAgent.EndEpisode();
                ResetScene();
            }

            EnemyList.RemoveAll( x => !x);

            if (EnemyList.Count == 0)
            {
                ResolveEvent(Event.killedAllTargets);
            }

            shootairAgent.AddReward(-2e-5f);
        }

        public void ResetScene()
        {
            resetTimer = 0;

            agent.transform.position = new Vector3(0, 0, 0);
            agent.transform.eulerAngles = new Vector3(0, 0, 0);

            agent.GetComponent<Rigidbody2D>().velocity = default;

            ResetEnemies();
        }

        void ResetEnemies()
        {
            EnemyList.Clear(); // clear list of enemies

            // kill previous instances of enemies
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.transform.name == "StandardEnemy(Clone)" || obj.transform.name == "DifficultEnemy(Clone)" || obj.transform.name == "FastEnemy(Clone)")
                {
                    Destroy(obj);
                }
            }

            List<int> enemyCount = environmentSettings.waves[currentWave];

            if (enemyCount[0] > 0)
            {
                spawn(standardEnemyPrefab, enemyCount[0], 11, 2.5f);
            }

            if (enemyCount[1] > 0)
            {
                spawn(fastEnemyPrefab, enemyCount[1], 7, -8);
            }

            if (enemyCount[2] > 0)
            {
                spawn(difficultEnemyPrefab, enemyCount[2], -8, -8);
            }
        }

        void spawn(GameObject prefab, int quant, float xOffset, float yOffset)
        {
            // Spawn enemies in spawn area
            for (int i = 1; i <= quant; i++)
            {
                GameObject newGO = Instantiate(prefab, new Vector3(xOffset * Random.value, yOffset, 0f), Quaternion.Euler(0f, 0f, Random.Range(0.0f, 360.0f)));
                newGO.transform.parent = area.transform;
                EnemyList.Add(newGO);
            }
        }
    }
}
