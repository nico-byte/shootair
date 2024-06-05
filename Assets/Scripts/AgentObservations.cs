using System.Collections.Generic;
using UnityEngine;

namespace ShootAirRLAgent
{
    public class AgentObservations : MonoBehaviour
    {
        public Dictionary<string, float> observations { get; set; } = new Dictionary<string, float>
        {
            {"velocity_x", 0f},
            {"velocity_y", 0f},
            {"shotAvailable", 0f},
            {"distanceEnemy", 0f},
            {"directionEnemy", 0f},
            {"velocity_xEnemy", 0f},
            {"velocity_yEnemy", 0f},
            {"healthEnemy", 0f},
            {"enemiesLeft", 0f},
        };
    }
}