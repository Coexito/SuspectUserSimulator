using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    [SerializeField] private GameObject honestPrefab;
    [SerializeField] private GameObject traitorPrefab;

    // World variables
    [SerializeField] [Header("Total tasks needed to win:")] private float TOTAL_TASKS = 20f; //Number of tasks needed to be done by honest agents

    [Header("Total agents:")]
    [SerializeField] private float totalHonestAgents = 5f; 
    
    [SerializeField] private float totalTraitorAgents = 2f;

    private int tasksDone = 0;

    [HideInInspector] public bool sabotageHappening;

    // Data structures
    public List<GameObject> agents;
    [SerializeField] private List<HonestBehaviour> agentsWaitingForTask;
    public List<Vector3> availableTasks;
    
    void Awake()
    {
        instance = this;
        availableTasks = new List<Vector3>();
        agentsWaitingForTask = new List<HonestBehaviour>();
    }

    private void Start() {
        SpawnAgents();
    }

    private void Update() {
        AssignTasks();


        if(Input.GetKeyDown(KeyCode.Space))
            KillAgent(agents[0]);
        else if(Input.GetKeyDown(KeyCode.V))
            StartVotation();
        else if(Input.GetKeyDown(KeyCode.F))
            FinishVotation();

    }

    private void SpawnAgents()
    {
        for(int i = 0; i < totalHonestAgents; i++)
        {
            agents.Add(Instantiate(honestPrefab, new Vector3(-12.1f, 6.9f, 16.7f), Quaternion.identity));
        }

        for (int i = 0; i < totalTraitorAgents; i++)
        {
            agents.Add(Instantiate(traitorPrefab, new Vector3(-12.1f, 6.9f, 10f), Quaternion.identity));
        }
    }

    private void StartVotation()
    {
        foreach (GameObject ag in agents)
            ag.GetComponent<Agent>().StartVote();

        agentsWaitingForTask.Clear();
    }

    private void FinishVotation()
    {
        foreach (GameObject ag in agents)
            ag.GetComponent<Agent>().FinishVote();
    }

    private void KillAgent(GameObject ag)
    {
        ag.GetComponent<Agent>().Die();
        agents.Remove(ag);
        agentsWaitingForTask.Remove(ag.GetComponent<HonestBehaviour>());
    }

    public void TakeTask(Vector3 task)
    {
        availableTasks.Remove(task);
    }

    public void IWantATask(HonestBehaviour agent)
    {
        agentsWaitingForTask.Add(agent);
    }

    private void AssignTasks()
    {
        // FIFO list
        for(int i = 0; i < availableTasks.Count; i++)
        {
            for(int j = 0; j < agentsWaitingForTask.Count; j++)
            {
                agentsWaitingForTask[j].setTask(availableTasks[i]);
                agentsWaitingForTask[j].TaskFound();

                agentsWaitingForTask.Remove(agentsWaitingForTask[j]);
                availableTasks.Remove(availableTasks[i]);

                break;
            }
        }
        
    }

    public float GetTotalTasks()
    {
        return TOTAL_TASKS;
    }

    public float GetTotalHonestAgents()
    {
        return totalHonestAgents;
    }

    public void TaskDone()
    {
        tasksDone++;
    }

    public float GetTasksDone()
    {
        return (float) tasksDone;
    }

    public float GetTotalTraitorAgents()
    {
        return totalTraitorAgents;
    }
}
