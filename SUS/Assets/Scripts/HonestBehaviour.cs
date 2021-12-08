using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;


public class HonestBehaviour : MonoBehaviour
{
    private StateMachineEngine generalFSM;
    private StateMachineEngine defaultFSM;
    private BehaviourTreeEngine workBT;

    public Agent thisAgent;
    [SerializeField] [Header("Agent speed:")] private float defaultSpeed = 5f;
    [SerializeField] private float distanceToRandomWalk = 50f;

    private NavMeshAgent agent;
    [SerializeField] private Vector3 currentTask = Vector3.zero;
    private bool taskFound = false;
    private bool vote = false;
    private bool killed = false;
    private bool notWorking = true;

    [SerializeField] private float timeWorking = 5f;

    private SpriteStateController spriteStateController; // To change the state sprite

    void Awake()
    {
        // Creates the object that represents this agent & has data structures
        thisAgent = GetComponent<Agent>();
        thisAgent.setSpeed(defaultSpeed);

        agent = GetComponent<NavMeshAgent>(); // Gets the navmeshagent
        agent.speed = thisAgent.getSpeed();

        //Work Behaviour Tree
        workBT = new BehaviourTreeEngine(true);

        // Default submachine for Working
        defaultFSM = new StateMachineEngine(BehaviourEngine.IsASubmachine);
        //General FSM
        generalFSM = new StateMachineEngine(BehaviourEngine.IsNotASubmachine);

        spriteStateController = GetComponent<SpriteStateController>();

        CreateBT();
        CreateFMS();
    }

    private void Start() 
    {
        SetTextName();
    }

    void Update()
    {
        workBT.Update();
        defaultFSM.Update();
        generalFSM.Update();

    }

    private void CreateFMS()
    {

        // Default substates
        State wanderState = defaultFSM.CreateEntryState("wander", Wander);
        State workState = defaultFSM.CreateSubStateMachine("work", workBT);

        // Perceptions
        ValuePerception	task = defaultFSM.CreatePerception<ValuePerception>(() => taskFound == true);
        BehaviourTreeStatusPerception taskDone = defaultFSM.CreatePerception<BehaviourTreeStatusPerception>(workBT, ReturnValues.Succeed);
        BehaviourTreeStatusPerception votationCalled = defaultFSM.CreatePerception<BehaviourTreeStatusPerception>(workBT, ReturnValues.Failed);
        BehaviourTreeStatusPerception killedWorking = defaultFSM.CreatePerception<BehaviourTreeStatusPerception>(workBT, ReturnValues.Failed);

        // Transitions
        defaultFSM.CreateTransition("task found", wanderState, task, workState);
        workBT.CreateExitTransition("BT done", workState, taskDone, wanderState);
        workBT.CreateExitTransition("votation called", workState, votationCalled, wanderState);
        workBT.CreateExitTransition("killed working", workState, killedWorking, wanderState);


        // States
        State initialState = generalFSM.CreateEntryState("idle");
        State defaultState = generalFSM.CreateSubStateMachine("default", defaultFSM);
        State votingState = generalFSM.CreateState("vote", Vote);
        State deadState = generalFSM.CreateState("dead", Die);

        // Perceptions
        Perception born = generalFSM.CreatePerception<TimerPerception>(0.25f);  // Waits a quarter of a second until they do something
        Perception voteCalled = generalFSM.CreatePerception<ValuePerception>(() => vote == true);
        Perception voteFinished = generalFSM.CreatePerception<ValuePerception>(() => vote == false);
        Perception youWereKilled = generalFSM.CreatePerception<ValuePerception>(() => killed == true);


        // Transitions
        generalFSM.CreateTransition("born", initialState, born, defaultState); // When born, enters the default state & starts looking for work
        defaultFSM.CreateExitTransition("vote called", wanderState, voteCalled, votingState);
        generalFSM.CreateTransition("vote finished", votingState, voteFinished, initialState);
        defaultFSM.CreateExitTransition("killed", wanderState, youWereKilled, deadState);

    }
    private void CreateBT()
    {
        SequenceNode rootNode = workBT.CreateSequenceNode("root", false);

        LeafNode walkToTask = workBT.CreateLeafNode("walk to task", () => WalkToTask(currentTask), () => isInObjective());
        LeafNode work = workBT.CreateLeafNode("work", () => WorkBT(), () => finishedWorking());

        rootNode.AddChild(walkToTask);
        rootNode.AddChild(work);

        workBT.SetRootNode(rootNode);
    }

    private void SetTextName()
    {
        TextMeshProUGUI nameTXT = transform.FindDeepChild("NameTXT").GetComponent<TextMeshProUGUI>();
        nameTXT.SetText(thisAgent.getAgentName());
    }


    #region FSMActions
    private void Wander()
    {
        notWorking = true;
        spriteStateController.SetStateIcon("wander");   // the state without caps!!

        this.GetComponentInParent<Renderer>().material.SetColor("_Color", Color.blue);
        SceneController.instance.IWantATask(this);  // Asks for a task

        agent.speed = thisAgent.getSpeed(); // Sets the default speed

        agent.SetDestination(GetRandomPoint(transform.position, distanceToRandomWalk));  // Walks randomly until given a task        
    }

    // Get Random Point on a Navmesh surface
    private Vector3 GetRandomPoint(Vector3 center, float maxDistance) {
        // Get Random Point inside Sphere which position is center, radius is maxDistance
        Vector3 randomPos = Random.insideUnitSphere * maxDistance + center;

        NavMeshHit hit; // NavMesh Sampling Info Container
        
        // from randomPos find a nearest point on NavMesh surface in range of maxDistance
        NavMesh.SamplePosition(randomPos, out hit, maxDistance, NavMesh.AllAreas);

        return hit.position;
    }

    public void setTask(Vector3 task)
    {
        this.currentTask = task;
    }

    public void TaskFound()
    {
        taskFound = true;
    }

    private void Vote()
    {
        /*
            Cuando se vota, todos los agentes se paralizan hasta que acabe la votación.
            Se muestra por pantalla el proceso de votación a través de una interfaz
            Al acabar la votación, se vuelve dentro de defaultFSM (vuelve a buscar trabajo)
            
            Hay que asegurarse de que el agente se para, pq si no el navmesh puede dar problemas
        */
        Debug.Log("Voting...");

        spriteStateController.SetStateIcon("vote");
        SceneController.instance.DeleteAgentsWaitingForTask(this);

        // Dismisses his task
        taskFound = false;  
        currentTask = Vector3.zero;
        agent.speed = 0;
        agent.SetDestination(transform.position);
        this.GetComponentInParent<Renderer>().material.SetColor("_Color", Color.white);

        /*
            Vote random agent (TO BE CHANGED)
            _________________________________
        */
        // Random agent
        int r = Random.Range(0, SceneController.instance.agents.Count);
        Agent agVoted = SceneController.instance.agents[r].GetComponent<Agent>();
        //Debug.Log(honest.thisAgent.getAgentName());
        //Debug.Log(ag.getAgentName());

        // Votes the agent
        SceneController.instance.VoteAgent(thisAgent, agVoted);

        /*
           _________________________________
        */
    }

    public void FireVote()
    {
        vote = true;
    }

    public void FireWander()
    {
        vote = false;
    }

    private void Die()
    {
        /*
            Cuando el agente muere, se queda quieto en el sitio e informa al mundo sobre ello.
            El mundo guarda su posición (donde ha muerto) y quién le ha matado.
            Se sustituye su modelo por un prefab de cadáver.

            Hay que asegurarse de que el agente se para, pq si no el navmesh puede dar problemas
        */
        Debug.Log("Im dead :(");
        spriteStateController.SetStateIcon("die");
        this.GetComponentInParent<CapsuleCollider>().enabled = false;
        agent.SetDestination(transform.position);
        this.GetComponentInParent<Renderer>().material.SetColor("_Color", Color.black);
        //Destroy(this.gameObject);
    }

    public void FireDie()
    {
        killed = true;
    }
    #endregion

    #region BTActions
    private void WalkToTask(Vector3 coords)
    {
        // Resets the bools
        taskFound = false;
        spriteStateController.SetStateIcon("go");
        agent.SetDestination(coords);
    }

    private void WorkBT()
    {
        spriteStateController.SetStateIcon("work");
        StartCoroutine(TimerWork());        
    }

    private IEnumerator TimerWork()
    {
        // Stops the agent until he finishes the task        
        agent.speed = 0;
        yield return new WaitForSeconds(timeWorking);
        notWorking = true;
        agent.speed = thisAgent.getSpeed();
    }
    #endregion

    #region BTEvaluationFunctions
    private ReturnValues isInObjective()
    {
        if (vote || killed)
            return ReturnValues.Failed;
        else
        {
            // Checks if agent position is task position
            if (Vector3.Distance(this.transform.position, currentTask) < 3)
            {
                notWorking = false;
                return ReturnValues.Succeed;
            }
            else
                return ReturnValues.Running;
        }        
    }

    private ReturnValues finishedWorking()
    {
        if (vote || killed)
            return ReturnValues.Failed;
        else
        {
            if (notWorking)
            {
                SceneController.instance.TaskDone();
                currentTask = Vector3.zero;
                return ReturnValues.Succeed;
            }
            else
                return ReturnValues.Running;
        }
    }
    #endregion
}
