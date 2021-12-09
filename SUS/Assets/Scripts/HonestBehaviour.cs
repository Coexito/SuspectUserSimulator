using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;


public class HonestBehaviour : MonoBehaviour
{
    private StateMachineEngine generalFSM;
    private StateMachineEngine defaultFSM;
    private BehaviourTreeEngine workBT;

    public HonestAgent thisAgent;
    [SerializeField] [Header("Agent speed:")] private float defaultSpeed = 5f;
    [SerializeField] private float distanceToRandomWalk = 100f;

    private NavMeshAgent agent;
    private Animator animator;
    [SerializeField] private Vector3 currentTask = Vector3.zero;
    private bool taskFound = false;
    private bool vote = false;
    private bool killed = false;
    private bool notWorking = true;
    private bool emergency = false;
    private bool sabotage = false;

    [SerializeField] private float timeWorking = 5f;

    private SpriteStateController spriteStateController; // To change the state sprite

    void Awake()
    {
        // Creates the object that represents this agent & has data structures
        thisAgent = GetComponent<HonestAgent>();
        thisAgent.setSpeed(defaultSpeed);

        agent = GetComponent<NavMeshAgent>(); // Gets the navmeshagent
        agent.speed = thisAgent.getSpeed();

        animator = GetComponent<Animator>();

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

        if(GetComponent<Rigidbody>().velocity == Vector3.zero)    // If the agent stays in place
            animator.SetBool("isWalking", false);
            
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
        BehaviourTreeStatusPerception deadAgentFoundWk = defaultFSM.CreatePerception<BehaviourTreeStatusPerception>(workBT, ReturnValues.Failed);
        BehaviourTreeStatusPerception sabotageWorking = defaultFSM.CreatePerception<BehaviourTreeStatusPerception>(workBT, ReturnValues.Failed);

        // Transitions
        defaultFSM.CreateTransition("task found", wanderState, task, workState);
        workBT.CreateExitTransition("BT done", workState, taskDone, wanderState);
        workBT.CreateExitTransition("votation called", workState, votationCalled, wanderState);
        workBT.CreateExitTransition("killed working", workState, killedWorking, wanderState);
        workBT.CreateExitTransition("dead agent working", workState, deadAgentFoundWk, wanderState);
        workBT.CreateExitTransition("sabotage working", workState, sabotageWorking, wanderState);


        // States
        State initialState = generalFSM.CreateEntryState("idle");
        State defaultState = generalFSM.CreateSubStateMachine("default", defaultFSM);
        State votingState = generalFSM.CreateState("vote", Vote);
        State deadState = generalFSM.CreateState("dead", Die);
        State emergencyState = generalFSM.CreateState("emergency", Emergency);
        State sabotageState = generalFSM.CreateState("sabotage", GoToSabotage);

        // Perceptions
        Perception born = generalFSM.CreatePerception<TimerPerception>(0.25f);  // Waits a quarter of a second until they do something
        Perception voteCalled = generalFSM.CreatePerception<ValuePerception>(() => vote == true);
        Perception voteFinished = generalFSM.CreatePerception<ValuePerception>(() => vote == false);
        Perception youWereKilled = generalFSM.CreatePerception<ValuePerception>(() => killed == true);
        Perception deadAgentFound = generalFSM.CreatePerception<ValuePerception>(() => emergency == true);
        Perception sabotageFired = generalFSM.CreatePerception<ValuePerception>(() => sabotage == true);
        Perception sabotageEnded = generalFSM.CreatePerception<ValuePerception>(() => sabotage == false);


        // Transitions
        generalFSM.CreateTransition("born", initialState, born, defaultState); // When born, enters the default state & starts looking for work
        defaultFSM.CreateExitTransition("vote called", wanderState, voteCalled, votingState);
        generalFSM.CreateTransition("i called votation", emergencyState, voteCalled, votingState);
        generalFSM.CreateTransition("vote finished", votingState, voteFinished, initialState);
        defaultFSM.CreateExitTransition("killed", wanderState, youWereKilled, deadState);
        defaultFSM.CreateExitTransition("dead agent", wanderState, deadAgentFound, emergencyState);
        defaultFSM.CreateExitTransition("sabotage fired", wanderState, sabotageFired, sabotageState);
        generalFSM.CreateTransition("sabotage ended", sabotageState, sabotageEnded, initialState);

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
        animator.SetBool("isWalking", true);

        SceneController.instance.IWantATask(this);  // Asks for a task

        agent.speed = thisAgent.getSpeed(); // Sets the default speed

        // Walks randomly until given a task 
        Vector3 randomPos = GetRandomPoint(transform.position, distanceToRandomWalk);
        agent.SetDestination(randomPos);
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
        spriteStateController.SetStateIcon("vote");

        animator.SetBool("isWalking", false);
        animator.SetBool("isWorking", false);

        SceneController.instance.DeleteAgentsWaitingForTask(this);

        // Dismisses his task
        taskFound = false;  
        currentTask = Vector3.zero;
        agent.speed = 0;
        agent.SetDestination(transform.position);

        /*
            Vote random agent (TO BE CHANGED)
            _________________________________
        */
        // Random agent
        Agent agVoted=thisAgent.GetMostSuspiciousAgent();

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
        spriteStateController.SetStateIcon("die");
        thisAgent.GetActualRoom().AgentKilledInRoom((HonestAgent) thisAgent);
        this.GetComponentInParent<CapsuleCollider>().enabled = false;
        animator.SetTrigger("Die");
        agent.SetDestination(transform.position);
    }

    public void FireDie()
    {
        killed = true;
    }

    private void Emergency()
    {
        spriteStateController.SetStateIcon("alarm");
        taskFound = false;
        currentTask = Vector3.zero;
        agent.SetDestination(SceneController.instance.EMERGENCY_POINT);
        StartCoroutine(IsInEmergencyPoint());
    }

    private IEnumerator IsInEmergencyPoint()
    {
        for (; ; )
        {
            if (Vector3.Distance(this.transform.position, SceneController.instance.EMERGENCY_POINT) < 3)
            {
                emergency = false;
                StartCoroutine(SceneController.instance.StartVotation());
                break;
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    public void FireSabotage()
    {
        sabotage = true;
    }

    public void EndSabotage()
    {
        sabotage = false;
    }

    private void GoToSabotage()
    {
        int itGoes = Random.Range(0, 100);

        if(itGoes < 60) // Posibility to go fix the sabotage
        {
            spriteStateController.SetStateIcon("sabotage");
            taskFound = false;
            currentTask = Vector3.zero;
            Vector3 sP = thisAgent.GetSabotagePoint();
            agent.SetDestination(sP);
            StartCoroutine(IsInSabotagePoint(sP));
        }
        else
        {
            sabotage = false;
        }        
    }

    private IEnumerator IsInSabotagePoint(Vector3 sP)
    {        
        for (; ; )
        {
            if (Vector3.Distance(this.transform.position, sP) < 3.5)
            {
                StartCoroutine(FixingSabotage());
                break;
            }
            
            if (!sabotage)
            {
                break;
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    private IEnumerator FixingSabotage()
    {      
        agent.speed = 0;
        yield return new WaitForSeconds(timeWorking);
        SceneController.instance.EndSabotage();
        agent.speed = thisAgent.getSpeed();
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

        animator.SetBool("isWalking", false);
        animator.SetBool("isWorking", true);

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
        if (vote || killed || emergency || sabotage)
            return ReturnValues.Failed;
        else
        {
            // Checks if agent position is task position
            if (Vector3.Distance(this.transform.position, currentTask) < 3.5f)
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
        if (vote || killed || emergency || sabotage)
            return ReturnValues.Failed;
        else
        {
            if (notWorking)
            {
                SceneController.instance.TaskDone();
                currentTask = Vector3.zero;
                animator.SetBool("isWorking", false);
                return ReturnValues.Succeed;
            }
            else
                return ReturnValues.Running;
        }
    }
    #endregion

    #region Setters&Getters
    public bool Killed { get => killed; set => killed = value; }
    #endregion

}
