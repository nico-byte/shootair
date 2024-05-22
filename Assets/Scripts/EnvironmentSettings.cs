using System.Collections.Generic;
using UnityEngine;

namespace ShootAirRLAgent
{
    public class EnvironmentSettings : MonoBehaviour
    {
        public List<List<int>> waves { get; set; } = new List<List<int>>
        {
            new List<int> {4, 0, 0},
            new List<int> {6, 0, 0},
            new List<int> {8, 0, 4},
            new List<int> {9, 1, 0},
            new List<int> {10, 1, 0},
            new List<int> {12, 1, 0},
            new List<int> {13, 1, 1},
            new List<int> {15, 1, 1},
            new List<int> {17, 1, 1},
            new List<int> {17, 2, 2},
            new List<int> {17, 2, 4},
            new List<int> {20, 2, 4},
            new List<int> {21, 3, 5},
            new List<int> {22, 4, 6},
            new List<int> {25, 4, 6},
            new List<int> {25, 6, 8},
            new List<int> {30, 6, 8},
            new List<int> {30, 8, 10},
            new List<int> {30, 8, 13},
            new List<int> {33, 10, 15},
            new List<int> {35, 10, 15},
            new List<int> {37, 11, 17},
            new List<int> {37, 12, 19},
            new List<int> {40, 12, 19},
            new List<int> {40, 13, 20},
            new List<int> {40, 15, 20},
            new List<int> {43, 16, 22},
            new List<int> {45, 17, 24},
            new List<int> {47, 18, 26},
            new List<int> {48, 19, 27},
            new List<int> {51, 20, 28},
            new List<int> {53, 22, 28},
            new List<int> {55, 22, 28},
            new List<int> {55, 23, 29},
            new List<int> {57, 24, 29},
            new List<int> {60, 25, 30}
        };
    }
}