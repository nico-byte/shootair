using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ShootAirRLAgent
{
    public class UIManager : MonoBehaviour
    {
        // OBJECTS
        AgentObservations agentObservations;

        // DEBUG PRINTS
        [SerializeField]
        private TextMeshProUGUI velocityXText;
        [SerializeField]
        private TextMeshProUGUI velocityYText;
        [SerializeField]
        private TextMeshProUGUI enemyVelocityXText;
        [SerializeField]
        private TextMeshProUGUI enemyVelocityYText;
        [SerializeField]
        private TextMeshProUGUI shotAvailableText;
        [SerializeField]
        private TextMeshProUGUI distanceEnemyText;
        [SerializeField]
        private TextMeshProUGUI directionEnemyText;
        [SerializeField]
        private TextMeshProUGUI healthEnemyText;
        
        // Start is called before the first frame update
        void Start()
        {
            agentObservations = FindObjectOfType<AgentObservations>();
        }
    
        void FixedUpdate()
        {
            Dictionary<string, float> observationValues = agentObservations.observations;
            UpdateVelocityX(MathF.Round(observationValues["velocity_x"], 3));
            UpdateVelocityY(MathF.Round(observationValues["velocity_y"], 3));
            UpdateShotAvailable(MathF.Round(observationValues["shotAvailable"], 3));
            UpdateDistanceEnemy(MathF.Round(observationValues["distanceEnemy"], 3));
            UpdateDirectionEnemy(MathF.Round(observationValues["directionEnemy"], 3));
            UpdateEnemyVelocityX(MathF.Round(observationValues["velocity_xEnemy"], 3));
            UpdateEnemyVelocityY(MathF.Round(observationValues["velocity_yEnemy"], 3));
            UpdateHealthEnemy(MathF.Round(observationValues["healthEnemy"], 3));
        }
    
        // Update is called once per frame
        private void UpdateVelocityX(float velocityX)
        {
            velocityXText.text = "velocity.x = " + velocityX.ToString();
        }
        
        private void UpdateVelocityY(float velocityY)
        {
            velocityYText.text = "velocity.y = " + velocityY.ToString();
        }

        private void UpdateEnemyVelocityX(float velocityX)
        {
            enemyVelocityXText.text = "velocity.x = " + velocityX.ToString();
        }
        
        private void UpdateEnemyVelocityY(float velocityY)
        {
            enemyVelocityYText.text = "velocity.y = " + velocityY.ToString();
        }
    
        private void UpdateShotAvailable(float shotAvailable)
        {
            shotAvailableText.text = "shotAvailable = " + shotAvailable.ToString();
        }
    
        private void UpdateDistanceEnemy(float distance)
        {
            distanceEnemyText.text = "distance = " + distance.ToString();
        }
    
        private void UpdateDirectionEnemy(float direction)
        {
            directionEnemyText.text = "direction = " + direction.ToString();
        }
        
        private void UpdateHealthEnemy(float health)
        {
            healthEnemyText.text = "health = " + health.ToString();
        }
    }
}
