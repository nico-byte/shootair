using UnityEngine;

namespace ShootAirRLAgent
{
    public class AgentSettings : MonoBehaviour
    {
        public float moveSpeed { get; } = 5f;
        public float turnSpeed { get; } = 60f;
        public float fireRate { get; set; } = 0.5f;
        public bool autoShoot { get; set; } = false;
        public bool selfplay { get; set; } = true;
        public float fireTimer { get; set; }
        public float maxVelocity { get; set; } = 10f;
        public float maxDistance { get; set; } = 65f;
        public float maxHealth { get; set; } = 150f;
        public string weaponEquipped { get; set; } = "shotgun"; // pistol | rifle | shotgun
    }
}