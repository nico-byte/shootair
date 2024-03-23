using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class EnvironmentSettings : MonoBehaviour
{
    public List<List<int>> waves = new List<List<int>>
    {
        new List<int> {3, 0, 0},
        new List<int> {5, 1, 0},
        new List<int> {5, 3, 1}
    };
}