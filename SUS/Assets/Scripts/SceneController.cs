using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Variables to start the simulation
    public static SceneController instance;
    [SerializeField] private GameObject honestPrefab;
    [SerializeField] private GameObject traitorPrefab;

    // World variables
    [Header("Total tasks needed to win:")] private float TOTAL_TASKS; //Number of tasks needed to be done by honest agents

    [Header("Total agents:")]
    private float totalHonestAgents; 
    private float totalTraitorAgents;
    private int tasksDone;

    [HideInInspector] public bool sabotageHappening;
    private bool gameWasSabotaged = false;    

    // Data structures
    MainMenuData dataMenu;

    [HideInInspector] public List<GameObject> agents;
    private List<HonestBehaviour> agentsWaitingForTask;
    [HideInInspector] public List<Vector3> availableTasks;
    private List<Agent> votesForAgents;

    [HideInInspector] public Vector3 EMERGENCY_POINT;

    // UI variables
    [SerializeField] private GameObject canvas;
    private GameObject votePanel;
    private TextMeshProUGUI voteLogs;
    private GameObject gameFinishPanel;
    private TextMeshProUGUI resultsLogs;
    private TextMeshProUGUI honestHUDNumber, traitorHUDNumber, tasksHUDNumber;
    
    void Awake()
    {
        instance = this;
        availableTasks = new List<Vector3>();
        agentsWaitingForTask = new List<HonestBehaviour>();
       
        dataMenu = GameObject.Find("MainMenuData").GetComponent<MainMenuData>();
        totalHonestAgents = dataMenu.Honests;
        totalTraitorAgents = dataMenu.Traitors;
        TOTAL_TASKS = dataMenu.Tasks;

        votesForAgents = new List<Agent>();
        agents = new List<GameObject>();

        // Gets the UI elements
        canvas.SetActive(true);
        votePanel = canvas.transform.FindDeepChild("VotePanel").gameObject;
        voteLogs = canvas.transform.FindDeepChild("VoteLog").GetComponent<TextMeshProUGUI>();

        gameFinishPanel = canvas.transform.FindDeepChild("GameFinishesPanel").gameObject;
        resultsLogs = canvas.transform.FindDeepChild("ResultsLog").GetComponent<TextMeshProUGUI>();

        honestHUDNumber = canvas.transform.FindDeepChild("HonestNumberTXT").GetComponent<TextMeshProUGUI>();
        traitorHUDNumber = canvas.transform.FindDeepChild("TraitorNumberTXT").GetComponent<TextMeshProUGUI>();
        tasksHUDNumber = canvas.transform.FindDeepChild("TasksNumberTXT").GetComponent<TextMeshProUGUI>();
    }

    private void Start() 
    {
        SpawnAgents();
        setAgentsInfo();
        votePanel.SetActive(false);
        gameFinishPanel.SetActive(false);
    }

    private void Update() 
    {
        UpdateHUD(); // Updates the HUD with current values

        AssignTasks();  // Assigns tasks to the agents waiting for it

        CheckGameStatus(); // Checks if the simulation has to end or continue

        // -------- PARA PRUEBAS, BORRAR LUEGO ------------
        if(Input.GetKeyDown(KeyCode.Space))
            KillAgent(agents[0]);
        else if (Input.GetKeyDown(KeyCode.V))
            StartCoroutine(StartVotation());
        else if (Input.GetKeyDown(KeyCode.F))
            FinishVotation();
        else if(Input.GetKeyDown(KeyCode.P))
            EndSimulation("All tasks done.\n\nHonest workers win.");
        //--------------------------------------------------

    }

    #region Game management

    private void CheckGameStatus()
    {
        if(tasksDone >= TOTAL_TASKS) // All needed tasks are done
        {
            // Honest agents win, simulation finishes
            EndSimulation("All tasks done.\n\nHonest workers win.");
        }
        else if(totalTraitorAgents == 0) // All traitors are ejected
        {
            // Honest agents win, simulation finishes
            EndSimulation("All traitors ejected.\n\nHonest workers win.");
        }
        else if(totalTraitorAgents > totalHonestAgents) // Traitors outnumber honests
        {
            // Traitor agents win, simulation finishes
            EndSimulation("There aren't enough honest workers.\n\nTraitors win.");
        }
        else if(gameWasSabotaged) // Game succesfully sabotaged
        {
            // Traitor agents win, simulation finishes
            EndSimulation("The sabotage succeeded.\n\nTraitors win.");
        }

    }

    private void EndSimulation(string finalMessage)
    {
        votePanel.SetActive(false);
        resultsLogs.SetText(finalMessage);
        gameFinishPanel.SetActive(true);
    }

    public void ResetSimulation()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitSimulation()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    private void UpdateHUD()
    {
        honestHUDNumber.SetText(totalHonestAgents.ToString());
        tasksHUDNumber.SetText(tasksDone.ToString());
        traitorHUDNumber.SetText(totalTraitorAgents.ToString());
    }

    #endregion

    #region Agents
    private void SpawnAgents()
    {
        try 
        {
            for(int i = 0; i < totalHonestAgents; i++)
            {
                GameObject h = Instantiate(honestPrefab, GetRandomPoint(new Vector3(-15f, 5f, 0f), 50f), Quaternion.identity);
                string name = "Agent" + (i+1);
                h.name = name;
                h.GetComponent<HonestAgent>().setAgentName(name);
                agents.Add(h);
            }
        }
        catch(UnassignedReferenceException e) { Debug.Log("Error spawning an honest agent."); }

        try
        {
            for (int i = 0; i < totalTraitorAgents; i++)
            {
                GameObject t = Instantiate(traitorPrefab, GetRandomPoint(new Vector3(-15f, 5f, 0f), 50f), Quaternion.identity);
                string name = "Traitor" + (i+1);
                t.name = name;
                t.GetComponent<TraitorAgent>().setAgentName(name);
                agents.Add(t);
            }
        }
        catch(UnassignedReferenceException e) { Debug.Log("Error spawning a traitor agent."); }
        
    }

    private Vector3 GetRandomPoint(Vector3 center, float maxDistance) {
        // Get Random Point inside Sphere which position is center, radius is maxDistance
        Vector3 randomPos = Random.insideUnitSphere * maxDistance + center;

        NavMeshHit hit; // NavMesh Sampling Info Container
        
        // from randomPos find a nearest point on NavMesh surface in range of maxDistance
        NavMesh.SamplePosition(randomPos, out hit, maxDistance, NavMesh.AllAreas);

        return hit.position;
    }

    #endregion

    public void StartSabotage(Vector3 sabotagePos)
    {
        foreach (GameObject ag in agents)
            ag.GetComponent<Agent>().StartSabotage(sabotagePos);
    }

    public void EndSabotage()
    {
        foreach (GameObject ag in agents)
            ag.GetComponent<Agent>().EndSabotage();
    }

    #region Voting functions
    public IEnumerator StartVotation()
    {
        voteLogs.SetText("");
        votePanel.SetActive(true);  // Opens the canvas

        foreach (GameObject ag in agents)
        {
            ag.GetComponent<Agent>().StartVote();
            /////////////////////////////////////////////////////////////////////////////// CAMBIAR 1 POR LA SALA
            if (ag.GetComponent<HonestAgent>() != null)
                ag.GetComponent<HonestAgent>().DecideVote(2);
        }
            
        yield return new WaitForSeconds(0.5f);

        // - Votation process -
        // Checks the most voted
        Agent agent = CheckMostVoted(); 

        // Ejecs the agent and writes in the logs
        EjectAgent(agent);
        voteLogs.text += "\n" + agent.getAgentName() + " was the most voted agent.\n\n";

        // Now the user has to finish the votation by pressing the button...
        // (executes FinishVotation)
    }

    public void DeleteAgentsWaitingForTask(HonestBehaviour ag)
    {
        agentsWaitingForTask.Remove(ag);
    }

    public void VoteAgent(Agent ag, Agent agVoted)
    {
        votesForAgents.Add(agVoted);

        //voteLogs.text += ag.getAgentName() + " has voted " + agVoted.getAgentName() + "\n";
        voteLogs.text += ag.getAgentName() + " has voted " + agVoted.getAgentName() + "\n";
    }

    private Agent CheckMostVoted()
    {
        Agent a = new Agent();
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
        
        return a;
    }



    private void EjectAgent(Agent ag)
    {
        if(ag != null)
        {
            agents.Remove(ag.gameObject);
            deleteAgentSus(ag.GetComponent<Agent>());
            ag.GetActualRoom().AgentKilledInRoom(ag);
            if(ag is HonestAgent)
            {
                agentsWaitingForTask.Remove(ag.GetComponent<HonestBehaviour>());
                totalHonestAgents--;
            }
            else if(ag is TraitorAgent)
            {
                totalTraitorAgents--;
            }
            Destroy(ag.gameObject);
        }
        
    }

    public void FinishVotation()
    {
        foreach (GameObject ag in agents)
            ag.GetComponent<Agent>().FinishVote();

        votePanel.SetActive(false);
        votesForAgents.Clear();
    }

    #endregion

    public void KillAgent(GameObject ag)
    {
        ag.GetComponent<Agent>().Die();
        agents.Remove(ag);
        deleteAgentSus(ag.GetComponent<Agent>());

        if (ag.GetComponent<Agent>() is HonestAgent)
        {
            agentsWaitingForTask.Remove(ag.GetComponent<HonestBehaviour>());
            totalHonestAgents--;
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

    #region SuspiciousInfo
    private void setAgentsInfo()
    {
        foreach(GameObject ag in agents)
        {
            if(ag.GetComponent<HonestAgent>()!=null)
                ag.GetComponent<HonestAgent>().GetAllAgents(agents);
        }
    }

    private void deleteAgentSus(Agent sus)
    {
        foreach (GameObject ag in agents)
        {
            if (ag.GetComponent<HonestAgent>() != null)
                ag.GetComponent<HonestAgent>().removeAgent(sus);
        }
    }
    #endregion
}
