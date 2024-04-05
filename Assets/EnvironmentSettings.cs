using System.Collections.Generic;
using UnityEngine;

namespace ShootAirRLAgent
{
    public class EnvironmentSettings : MonoBehaviour
    {
        public List<List<int>> waves { get; set; } = new List<List<int>>
        {
            new List<int> {3, 0, 0},
            new List<int> {5, 1, 0},
            new List<int> {5, 3, 1}
        };
    }
}