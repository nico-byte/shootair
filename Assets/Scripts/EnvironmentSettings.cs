using System.Collections.Generic;
using UnityEngine;

namespace ShootAirRLAgent
{
    public class EnvironmentSettings : MonoBehaviour
    {
        public List<List<int>> waves { get; set; } = new List<List<int>>
        {
            // {StandardEnemy(Slime), FastEnemy(Sceleton), DifficultEnemy(Vampire), Occultist3 (Boss1), Occultist4(Boss2)}
            new List<int> {4, 0, 0, 1, 1},
            new List<int> {6, 0, 0, 1, 0},
            new List<int> {8, 0, 4, 0, 0},
            new List<int> {9, 1, 0, 1, 0},
            new List<int> {10, 1, 0, 2, 0},
            new List<int> {12, 1, 0, 2, 0},
            new List<int> {13, 1, 1, 0, 1},
            new List<int> {15, 1, 1, 0, 1},
            new List<int> {17, 1, 1, 1, 1},
            new List<int> {17, 2, 2, 2, 1},
            new List<int> {17, 2, 4, 2, 2},
            new List<int> {20, 2, 4, 3, 3}
        };
    }
}