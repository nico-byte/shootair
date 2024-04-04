using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI velocityXText;
    public TextMeshProUGUI velocityYText;
    public TextMeshProUGUI shotAvailableText;
    public TextMeshProUGUI distanceEnemyText;
    public TextMeshProUGUI directionEnemyText;
    public TextMeshProUGUI velocityXEnemyText;
    public TextMeshProUGUI velocityYEnemyText;
    public TextMeshProUGUI healthEnemyText;
    AgentObservations agentObservations;
    private Dictionary<string, float> observations;
    // Start is called before the first frame update
    void Start()
    {
        agentObservations = FindObjectOfType<AgentObservations>();
    }

    void FixedUpdate()
    {
        Dictionary<string, float> observations = agentObservations.observations;
        UpdateVelocityX(MathF.Round(observations["velocity_x"], 3));
        UpdateVelocityY(MathF.Round(observations["velocity_y"], 3));
        UpdateShotAvailable(MathF.Round(observations["shotAvailable"], 3));
        UpdateDistanceEnemy(MathF.Round(observations["distanceEnemy"], 3));
        UpdateDirectionEnemy(MathF.Round(observations["directionEnemy"], 3));
        UpdateVelocityXEnemy(MathF.Round(observations["velocity_xEnemy"], 3));
        UpdateVelocityYEnemy(MathF.Round(observations["velocity_yEnemy"], 3));
        UpdateHealthEnemy(MathF.Round(observations["healthEnemy"], 3));
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

    private void UpdateHealthEnemy(float health)
    {
        healthEnemyText.text = "health = " + health.ToString();
    }
    
}
