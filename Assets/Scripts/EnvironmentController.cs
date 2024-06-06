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
        SoundEffectPlayer soundHandler;

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
        private Transform[] spawnpoints = null;

        private int resetTimer;
        [SerializeField]
        private int MaxEnvironmentSteps;
        private int currentWave = 0;
        private int streak;

        // Start is called before the first frame update
        void Start()
        {
            environmentSettings = FindObjectOfType<EnvironmentSettings>();
            soundHandler = FindObjectOfType<SoundEffectPlayer>();
            agentSettings = FindObjectOfType<AgentSettings>();

            area = Instantiate(trainingAreaPrefab, environment.position, environment.rotation);
            agent = GameObject.FindGameObjectWithTag("agent");

            agent.transform.parent = area.transform;

            Transform point1 = GameObject.Find("p1").transform;
            Transform point2 = GameObject.Find("p2").transform;
            Transform point3 = GameObject.Find("p3").transform;
            Transform point4 = GameObject.Find("p4").transform;
            Transform point5 = GameObject.Find("p5").transform;
            Transform point6 = GameObject.Find("p6").transform;
            Transform point7 = GameObject.Find("p7").transform;
            Transform point8 = GameObject.Find("p8").transform;
            Transform point9 = GameObject.Find("p9").transform;
            Transform point10 = GameObject.Find("p10").transform;
            Transform point11 = GameObject.Find("p11").transform;
            Transform point12 = GameObject.Find("p12").transform;
            Transform point13 = GameObject.Find("p13").transform;
            Transform point14 = GameObject.Find("p14").transform;
            Transform point15 = GameObject.Find("p15").transform;
            Transform point16 = GameObject.Find("p16").transform;
            Transform point17 = GameObject.Find("p17").transform;
            Transform point18 = GameObject.Find("p18").transform;
            Transform point19 = GameObject.Find("p19").transform;
            Transform point20 = GameObject.Find("p20").transform;
            Transform point21 = GameObject.Find("p21").transform;
            spawnpoints = new Transform[21] {
                point1,
                point2,
                point3,
                point4,
                point5,
                point6,
                point7,
                point8,
                point9,
                point10,
                point11,
                point12,
                point13,
                point14,
                point15,
                point16,
                point17,
                point18,
                point19,
                point20,
                point21
            };

            Transform refpoint1 = GameObject.Find("refpoint1").transform;
            Transform refpoint2 = GameObject.Find("refpoint2").transform;

            agentSettings.maxDistance = Vector2.Distance(refpoint1.position, refpoint2.position);

            ResetScene();
        }

        public void ResolveEvent(Event triggerEvent)
        {
            switch (triggerEvent)
            {
                case Event.hitOnTarget:

                    // apply reward to shootair agent
                    shootairAgent.AddReward(8e-5f);
                    soundHandler.playSound("enemy_damage");

                    break;

                case Event.collisionWithTarget:
                    // agent loses
                    shootairAgent.SetReward(-.9f);
                    soundHandler.playSound("agent_death");
                    soundHandler.playSound("game_over");

                    currentWave = 0;

                    // end episode
                    shootairAgent.EndEpisode();
                    ResetScene();
                    break;

                case Event.killedTarget:
                    // add reward for killing target
                    shootairAgent.AddReward(1.5e-3f);
                    soundHandler.playSound("enemy_death");

                    break;

                case Event.killedAllTargets:
                    if (currentWave >= environmentSettings.waves.Count - 1)
                    {
                        currentWave = 0;
                        shootairAgent.EndEpisode();
                        ResetScene();
                        break;
                        // HE WON ALL WAVES
                    }

                    // agent wins
                    shootairAgent.AddReward(.4f / environmentSettings.waves.Count);
                    soundHandler.playSound("wave_success");

                    // end episode

                    currentWave++;
                    ResetEnemies();
                    break;

                case Event.missedShot:

                    // apply reward to shootair agent
                    shootairAgent.AddReward(-1e-5f);

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

            EnemyList.RemoveAll(x => !x);

            if (EnemyList.Count == 0)
            {
                ResolveEvent(Event.killedAllTargets);
            }

            shootairAgent.AddReward(-1e-8f);
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
                spawn(standardEnemyPrefab, enemyCount[0]);
            }

            if (enemyCount[1] > 0)
            {
                spawn(fastEnemyPrefab, enemyCount[1]);
            }

            if (enemyCount[2] > 0)
            {
                spawn(difficultEnemyPrefab, enemyCount[2]);
            }
        }

        void spawn(GameObject prefab, int quant)
        {
            // Spawn enemies in spawn area
            for (int i = 1; i <= quant; i++)
            {
                int randomIdx = Random.Range(0, spawnpoints.Length);
                GameObject newGO = Instantiate(prefab, spawnpoints[randomIdx].position, Quaternion.Euler(0f, 0f, Random.Range(0.0f, 360.0f)));
                newGO.transform.parent = area.transform;
                EnemyList.Add(newGO);
            }
        }
    }
}
