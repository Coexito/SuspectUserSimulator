using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    // World variables
    public const float TOTAL_TASKS = 20f; //Number of tasks needed to be done by honest agents
    public const float TOTAL_AGENTS = 5f; //Total number of agents 
    public bool sabotageHappening;

    // Data structures
    public List<Agent> agents;
    public List<Vector3> workingPositions;

    void Awake()
    {
        instance = this;
    }

}
