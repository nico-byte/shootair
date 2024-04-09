using System.Collections.Generic;
using UnityEngine;

namespace ShootAirRLAgent
{
    public class EnvironmentSettings : MonoBehaviour
    {
        public List<List<int>> waves { get; set; } = new List<List<int>>
        {
            new List<int> {6, 0, 0},
            new List<int> {8, 4, 0},
            new List<int> {14, 6, 4},
            new List<int> {20, 10, 6}
        };
    }
}