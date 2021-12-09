using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class TraitorBehaviour : MonoBehaviour
{
    private StateMachineEngine defaultFSM;
    private StateMachineEngine generalFSM;

    [SerializeField] [Range(0, 20)] [Header("Cooldown time in seconds:")] private float cooldown = 0.5f;
    [SerializeField] [Header("Agent speed:")] private float defaultSpeed = 5f;
    [SerializeField] private float distanceToRandomWalk = 50f;

    private TraitorAgent thisAgent;
    private NavMeshAgent agent;
    [SerializeField] private Vector3 currentTask = Vector3.zero;

    [SerializeField] private float timeWorking = 5f;

    private bool vote = false;
    
    private SpriteStateController spriteStateController; // To change the state sprite


    private void Awake()
    {

        // Creates the object that represents this agent & has data structures
        thisAgent = GetComponent<TraitorAgent>();
        thisAgent.setSpeed(defaultSpeed);

        agent = GetComponent<NavMeshAgent>(); // Gets the navmeshagent
        agent.speed = thisAgent.getSpeed();

        generalFSM = new StateMachineEngine(true);
        defaultFSM = new StateMachineEngine();

        spriteStateController = GetComponent<SpriteStateController>();

        //General FSM
        CreateFSM();        
    }

    // Update is called once per frame
    void Update()
    {  
        generalFSM.Update();
        defaultFSM.Update();
    }

    private void Start() 
    {
        SetTextName();
    }

    private void SetTextName()
    {
        TextMeshProUGUI nameTXT = transform.FindDeepChild("NameTXT").GetComponent<TextMeshProUGUI>();
        nameTXT.SetText(thisAgent.getAgentName());
    }

    #region CreateMachines

    private void CreateFSM()
    {
        //Create submachine

        State wanderState = generalFSM.CreateEntryState("wander", Wander);
        State workState = generalFSM.CreateState("work", TakeDecision);

        TimerPerception cooldownEnded = generalFSM.CreatePerception<TimerPerception>(cooldown);
        generalFSM.CreateTransition("cooldown terminado", wanderState, cooldownEnded, workState);

        PushPerception decisionTaken = generalFSM.CreatePerception<PushPerception>();
        generalFSM.CreateTransition("decision tomada", workState, decisionTaken, wanderState);

        //Create supermachine

        // States
        State initialState = defaultFSM.CreateEntryState("idle");
        State generalState = defaultFSM.CreateSubStateMachine("default", generalFSM);
        State votingState = defaultFSM.CreateState("vote", Vote);

        // Perceptions
        Perception born = defaultFSM.CreatePerception<TimerPerception>(0.25f);
        Perception voteCalled = defaultFSM.CreatePerception<ValuePerception>(() => vote == true);
        Perception voteFinished = defaultFSM.CreatePerception<ValuePerception>(() => vote == false);


        // Transitions
        defaultFSM.CreateTransition("born", initialState, born, generalState); // When born, enters the default state & starts looking for work
        generalFSM.CreateExitTransition("vote called from wander", wanderState, voteCalled, votingState);
        generalFSM.CreateExitTransition("vote called from work", workState, voteCalled, votingState);
        defaultFSM.CreateTransition("vote finished", votingState, voteFinished, initialState);
    }

    #endregion

    #region EntryUSDataMethods

    //Returns 1 if there's 0, 2 or more honest agents with the traitor (must not kill). Returns 0 if there's only 1 honest agents with the traitor (can kill)
    private float Get2OrMoreAgentsInRoom()
    {
        int agentsInRoom = thisAgent.NumberOfHonestAgentsInRoom();
        if (agentsInRoom == 1)
        {
            thisAgent.SetVictim();
            return 0f;
        }
        else
            return 1f;
    }

    private float GetNumberOfAgentsInLastRoom()
    {
        return (float) thisAgent.NumberOfHonestAgentsInLastRoom();
    }

    //Returns 0 if a honest agent just left the room
    private float GetAgentLeft()
    {
        if (thisAgent.GetAgentLeft())
            return 0f;
        else
            return 1f;
    }

    #endregion

    #region UtilityActionsMethods

    private void Sabotage()
    {        
        spriteStateController.SetStateIcon("sabotage");
        generalFSM.Fire("decision tomada");
    }

    private void Kill()
    {
        spriteStateController.SetStateIcon("kill");
        thisAgent.GetVictim().gameObject.GetComponent<NavMeshAgent>().speed = 0;
        agent.speed *= 2;
        agent.SetDestination(thisAgent.GetVictim().gameObject.transform.position);
        StartCoroutine(KillObjective());             
    }

    private IEnumerator KillObjective()
    {
        for (; ; )
        {
            if (Vector3.Distance(this.transform.position, thisAgent.GetVictim().gameObject.transform.position) < 2.25f)
            {
                SceneController.instance.KillAgent(thisAgent.GetVictim().gameObject);                
                generalFSM.Fire("decision tomada");
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    private void Pretend()
    {
        WalkToTask();  
    }

    #endregion

    #region BTActions

    private void WalkToTask()
    {
        int taskSelected = Mathf.RoundToInt(Random.Range(0, TaskGenerator.instance.tasksCoords.Count - 1));
        currentTask = TaskGenerator.instance.tasksCoords[taskSelected];
        agent.SetDestination(currentTask);
        spriteStateController.SetStateIcon("go");

        StartCoroutine(IsInObjective());
    }

    private IEnumerator IsInObjective()
    {
        for(;;)
        {
            if (Vector3.Distance(this.transform.position, currentTask) < 3)
            {
                Work();
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    private void Work()
    {
        spriteStateController.SetStateIcon("work");
        StartCoroutine(TimerWork());
    }

    private IEnumerator TimerWork()
    {
        // Stops the agent until he finishes the task        
        agent.speed = 0;
        yield return new WaitForSeconds(timeWorking);
        agent.speed = thisAgent.getSpeed();
        currentTask = Vector3.zero;
        generalFSM.Fire("decision tomada");
    }

    #endregion    

    #region FSMActions

    private void Wander()
    {
        spriteStateController.SetStateIcon("think");
        agent.speed = thisAgent.getSpeed(); // Sets the default speed

        agent.SetDestination(GetRandomPoint(transform.position, distanceToRandomWalk));  // Walks randomly until given a task     
          
    }

    // Get Random Point on a Navmesh surface
    private Vector3 GetRandomPoint(Vector3 center, float maxDistance)
    {
        // Get Random Point inside Sphere which position is center, radius is maxDistance
        Vector3 randomPos = Random.insideUnitSphere * maxDistance + center;

        NavMeshHit hit; // NavMesh Sampling Info Container

        // from randomPos find a nearest point on NavMesh surface in range of maxDistance
        NavMesh.SamplePosition(randomPos, out hit, maxDistance, NavMesh.AllAreas);

        return hit.position;
    }

    private void Vote()
    {
        /*
            Cuando se vota, todos los agentes se paralizan hasta que acabe la votaci�n.
            Se muestra por pantalla el proceso de votaci�n a trav�s de una interfaz
        */

        // Dismisses his task
        currentTask = Vector3.zero;
        agent.speed = 0;
        agent.SetDestination(transform.position);
        spriteStateController.SetStateIcon("vote");
    }

    public void FireVote()
    {
        vote = true;
    }

    public void FireWander()
    {
        vote = false;
    }

    #endregion

    private void TakeDecision()
    {
        //Base factors (data received from the world)
        float tasksCompleted = SceneController.instance.GetTasksDone() / SceneController.instance.GetTotalTasks();
        float agentsInLastRoom = GetNumberOfAgentsInLastRoom() / SceneController.instance.GetTotalHonestAgents();

        float killingPossibility = Mathf.Abs(Get2OrMoreAgentsInRoom() - 1);
        float needToPretend = Get2OrMoreAgentsInRoom(); //Decisive factor
        float fear2 = GetAgentLeft();

        //Graph factors
        float fear1 = (-agentsInLastRoom) + 1;

        List<Point2D> points = new List<Point2D>();
        points.Add(new Point2D(0, 0));
        points.Add(new Point2D(0.15f, 0));
        points.Add(new Point2D(0.4f, 0.25f));
        points.Add(new Point2D(0.5f, 0.5f));
        points.Add(new Point2D(0.6f, 0.75f));
        points.Add(new Point2D(0.85f, 1));
        points.Add(new Point2D(1, 1));

        float returnValue = 0.0f;
        float x = tasksCompleted;

        for (int i = 0; i < points.Count; i++)
        {
            float xPoint = points[i].x;
            if (i == 0 && x < xPoint) { returnValue = points[i].y; break; };
            if ((i == points.Count - 1) && x > xPoint) { returnValue = points[i].y; break; };
            if (x == xPoint) { returnValue = points[i].y; break; }

            if (x > xPoint && x < points[i + 1].x)
            {
                returnValue = ((x - xPoint) / (points[i + 1].x - xPoint)) * (points[i + 1].y - points[i].y) + points[i].y;
                break;
            }
        }

        float riskToLose = returnValue; //Decisive factor

        //Fusion factors
        List<float> factors = new List<float>();
        factors.Add(fear1);
        factors.Add(fear2);

        List<float> weights = new List<float>();
        weights.Add(0.5f);
        weights.Add(0.5f);

        float sum = 0.0f;
        for (int i = 0; i < factors.Count; i++)
        {
            float factor = factors[i];

            sum += factor * weights[i];
        }

        float fearToBeDiscovered = sum;

        factors.Clear();
        factors.Add(riskToLose);
        factors.Add(fearToBeDiscovered);
        factors.Add(killingPossibility);

        weights.Clear();
        weights.Add(0.2f);
        weights.Add(0.3f);
        weights.Add(0.5f);

        sum = 0.0f;
        for (int i = 0; i < factors.Count; i++)
        {
            float factor = factors[i];

            sum += factor * weights[i];
        }

        float killingNeed = sum; //Decisive factor

        float[] decisiveFactors = { needToPretend, riskToLose, killingNeed };

        float decision = Mathf.Max(decisiveFactors);

        if(decision == needToPretend)
        {
            Pretend();
        }
        else if (decision == killingNeed)
        {
            Kill();
        }
        else if (decision == riskToLose)
        {
            Sabotage();
        }
    }
}
