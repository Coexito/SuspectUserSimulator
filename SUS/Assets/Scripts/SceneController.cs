using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    // World variables
    [SerializeField] [Header("Total tasks needed to win:")] private float TOTAL_TASKS = 20f; //Number of tasks needed to be done by honest agents

    [Header("Total agents:")]
    [SerializeField] private float totalHonestAgents = 5f; 
    [SerializeField] private float totalTraitorAgents = 2f; 

    [HideInInspector] public bool sabotageHappening;

    // Data structures
    public List<Agent> agents;
    public List<Vector3> workingPositions;

    void Awake()
    {
        instance = this;
    }

    public float GetTotalTasks()
    {
        return TOTAL_TASKS;
    }

    public float GetTotalHonestAgents()
    {
        return totalHonestAgents;
    }

    public float GetTotalTraitorAgents()
    {
        return totalTraitorAgents;
    }
}
