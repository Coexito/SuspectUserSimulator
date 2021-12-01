using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitorBehaviour : MonoBehaviour
{
    private UtilitySystemEngine killingUS;
    private const float TOTAL_TASKS = 20f; //Number of tasks needed to be done by honest agents
    private const float TOTAL_AGENTS = 5f; //Total number of agents 

    void Awake()
    {
        killingUS = new UtilitySystemEngine(true);

        //Base factors (data received from the world)
        Factor tasksCompleted = new LeafVariable(() => GetNumberOfTasksCompleted(), TOTAL_TASKS, 0f);
        Factor agentsInRoom = new LeafVariable(() => GetNumberOfAgentsInRoom(), TOTAL_AGENTS, 0f);
        Factor agentsInLastRoom = new LeafVariable(() => GetNumberOfAgentsInLastRoom(), TOTAL_AGENTS, 0f);
        Factor agentJustLeftRoom = new LeafVariable(() => GetAgentLeft(), 1f, 0f);

        //Graph factors

        //Fusion factors

        //Actions and decisions

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        killingUS.Update();
    }

    private float GetNumberOfTasksCompleted()
    {
        return 0f;
    }

    private float GetNumberOfAgentsInRoom()
    {
        return 0f;
    }

    private float GetNumberOfAgentsInLastRoom()
    {
        return 0f;
    }

    private float GetAgentLeft()
    {
        return 0f;
    }
}
