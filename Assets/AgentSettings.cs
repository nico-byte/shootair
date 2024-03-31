using UnityEngine;

public class AgentSettings : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 60f;
    public float fireRate = 0.5f;
    public bool autoShoot = false;
    public bool selfplay = true;
    public float fireTimer;
    public float maxVelocity = 10f;
}