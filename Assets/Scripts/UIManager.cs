using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ShootAirRLAgent
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI velocityXText;
        [SerializeField]
        private TextMeshProUGUI velocityYText;
        [SerializeField]
        private TextMeshProUGUI shotAvailableText;
        [SerializeField]
        private TextMeshProUGUI distanceEnemyText;
        [SerializeField]
        private TextMeshProUGUI directionEnemyText;
        [SerializeField]
        private TextMeshProUGUI velocityXEnemyText;
        [SerializeField]
        private TextMeshProUGUI velocityYEnemyText;
        //[SerializeField]
        // private TextMeshProUGUI timeLeftText;
        [SerializeField]
        private TextMeshProUGUI healthEnemyText;
        AgentObservations agentObservations;
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
            UpdateVelocityXEnemy(MathF.Round(observationValues["velocity_xEnemy"], 3));
            UpdateVelocityYEnemy(MathF.Round(observationValues["velocity_yEnemy"], 3));
            // UpdateTimeLeft(MathF.Round(observationValues["timeLeft"], 3));
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
    
        private void UpdateVelocityXEnemy(float velocityX)
        {
            velocityXEnemyText.text = "velocity.x = " + velocityX.ToString();
        }
    
        private void UpdateVelocityYEnemy(float velocityY)
        {
            velocityYEnemyText.text = "velocity.y = " + velocityY.ToString();
        }
        
        //private void UpdateTimeLeft(float timeLeft)
        //{
        //    timeLeftText.text = "timeLeft = " + timeLeft.ToString();
        //}
        
        private void UpdateHealthEnemy(float health)
        {
            healthEnemyText.text = "health = " + health.ToString();
        }
        
    }
}
