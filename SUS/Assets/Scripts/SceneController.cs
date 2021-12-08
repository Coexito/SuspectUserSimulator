using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

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
    
    private TextMeshProUGUI voteLogs;
    [SerializeField] GameObject canvas;
    

    // Data structures
    public List<GameObject> agents;
    [SerializeField] private List<HonestBehaviour> agentsWaitingForTask;
    public List<Vector3> availableTasks;
    private List<Agent> votesForAgents;
    
    void Awake()
    {
        instance = this;
        availableTasks = new List<Vector3>();
        agentsWaitingForTask = new List<HonestBehaviour>();
        voteLogs = GameObject.Find("VoteLog").GetComponent<TextMeshProUGUI>();
        votesForAgents = new List<Agent>();
        canvas = GameObject.Find("Canvas");
    }

    private void Start() {
        SpawnAgents();
        canvas.SetActive(false);
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
            GameObject h = Instantiate(honestPrefab, GetRandomPoint(new Vector3(-15f, 5f, 0f), 50f), Quaternion.identity);
            //string name ="Agent" + (i+1);
            //h.name = name;
            //h.GetComponent<HonestAgent>().setAgentName(name);
            agents.Add(h);
        }

        for (int i = 0; i < totalTraitorAgents; i++)
        {
            GameObject t = Instantiate(traitorPrefab, GetRandomPoint(new Vector3(-15f, 5f, 0f), 50f), Quaternion.identity);
            string name = "Agent" + (i + 1);
            t.name = name;
            t.GetComponent<HonestAgent>().setAgentName(name);
            agents.Add(t);
        }
    }

    private Vector3 GetRandomPoint(Vector3 center, float maxDistance) {
        // Get Random Point inside Sphere which position is center, radius is maxDistance
        Vector3 randomPos = Random.insideUnitSphere * maxDistance + center;

        NavMeshHit hit; // NavMesh Sampling Info Container
        
        // from randomPos find a nearest point on NavMesh surface in range of maxDistance
        NavMesh.SamplePosition(randomPos, out hit, maxDistance, NavMesh.AllAreas);

        return hit.position;
    }

    #region Voting functions
    private void StartVotation()
    {
        canvas.SetActive(true);  // Opens the canvas

        foreach (GameObject ag in agents)
            ag.GetComponent<Agent>().StartVote();

        agentsWaitingForTask.Clear();

    }

    public void VoteAgent(Agent ag, Agent agVoted)
    {
        votesForAgents.Add(agVoted);
        voteLogs.text += ag.getAgentName() + " has voted " + agVoted.getAgentName() + "\n";
        //voteLogs.SetText(ag.getAgentName() + " has voted " + agVoted.getAgentName() + "\n");
    }

    private Agent CheckMostVoted()
    {
        Agent a = new Agent(0f);
        int i = 0;

        // Searchs for the most voted agent in the list (most repeated one)
        foreach (Agent ag in votesForAgents)
        {
            int aux = 0;
            foreach (Agent localAgent in votesForAgents)
            {
                if(localAgent == ag)
                    aux++;
            }

            if(aux > i)
            {
                i = aux;
                a = ag;
            }
                
        }
        
        Debug.Log("Most voted:" + a.getAgentName());
        return a;
    }

    private void EjectAgent(Agent ag)
    {
        agents.Remove(ag.gameObject);

        if(ag is HonestAgent)
        {
            agentsWaitingForTask.Remove(ag.GetComponent<HonestBehaviour>());
        }

        Destroy(ag.gameObject);
    }

    public void FinishVotation()
    {
        // - Votation process -
        // Checks the most voted
        Agent agent = CheckMostVoted(); 

        // Ejecs the agent and writes in the logs
        EjectAgent(agent);

        // Now the user has to finish the votation by pressing the button...

        foreach (GameObject ag in agents)
            ag.GetComponent<Agent>().FinishVote();

        canvas.SetActive(false);
    }

    #endregion

    private void KillAgent(GameObject ag)
    {

        voteLogs.SetText(ag.GetComponent<Agent>().getAgentName() + " has been ejected.");
        ag.GetComponent<Agent>().Die();
        agents.Remove(ag);

        if(ag.GetComponent<Agent>() is HonestAgent)
        {
            agentsWaitingForTask.Remove(ag.GetComponent<HonestBehaviour>());
        }
    }

    #region Tasks functions
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

    public void TaskDone()
    {
        tasksDone++;
    }

    #endregion

    #region Getters for SceneController variables
    public float GetTotalTasks()
    {
        return TOTAL_TASKS;
    }

    public float GetTotalHonestAgents()
    {
        return totalHonestAgents;
    }

    public float GetTasksDone()
    {
        return (float) tasksDone;
    }

    public float GetTotalTraitorAgents()
    {
        return totalTraitorAgents;
    }

    #endregion
}
