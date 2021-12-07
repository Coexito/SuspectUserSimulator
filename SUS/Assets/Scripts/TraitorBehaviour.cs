using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TraitorBehaviour : MonoBehaviour
{
    private StateMachineEngine generalFSM;
    private UtilitySystemEngine killingUS;
    private BehaviourTreeEngine pretendBT;

    [SerializeField] [Range(0, 20)] [Header("Cooldown time in seconds:")] private int cooldown = 2;
    [SerializeField] [Header("Agent speed:")] private float defaultSpeed = 5f;

    private TraitorAgent thisAgent;
    private NavMeshAgent agent;
    [SerializeField] private Vector3 currentTask = new Vector3(-13.6f, 10.3f, 19.7f);

    [SerializeField] private float timeWorking = 5f;

    private float totalTasksDone;

    private void Awake()
    {
        totalTasksDone = 0f;

        // Creates the object that represents this agent & has data structures
        thisAgent = new TraitorAgent(defaultSpeed);

        agent = GetComponent<NavMeshAgent>(); // Gets the navmeshagent
        agent.speed = this.defaultSpeed;

        pretendBT = new BehaviourTreeEngine(true);
        killingUS = new UtilitySystemEngine(true);
        generalFSM = new StateMachineEngine();

        //Pretend Behaviour Tree
        CreatePretendBehaviourTree();        

        //Killing Utility System
        CreateKillingUtilitySystem();        

        //General FSM
        CreateGeneralFSM();        
    }

    // Update is called once per frame
    void Update()
    {  
        pretendBT.Update();
        killingUS.Update();
        generalFSM.Update();
    }

    void FixedUpdate()
    {
        totalTasksDone = SceneController.instance.GetTasksDone();
    }

    #region CreateMachines

    private void CreateGeneralFSM()
    {        
        State wanderState = generalFSM.CreateEntryState("wander", Wander);
        State workState = generalFSM.CreateSubStateMachine("work", killingUS);

        TimerPerception cooldownEnded = generalFSM.CreatePerception<TimerPerception>(cooldown);
        Perception decisionTaken = generalFSM.CreatePerception<Perception>();

        generalFSM.CreateTransition("cooldown terminado", wanderState, cooldownEnded, workState);
        killingUS.CreateExitTransition("decision tomada", workState, decisionTaken, wanderState);
    }

    private void CreateKillingUtilitySystem()
    {
        //Base factors (data received from the world)
        Factor tasksCompleted = new LeafVariable(() => totalTasksDone, SceneController.instance.GetTotalTasks(), 0f);
        Factor agentsInLastRoom = new LeafVariable(GetNumberOfAgentsInLastRoom, SceneController.instance.GetTotalHonestAgents(), 0f);

        Factor killingPossibility = new LeafVariable(() => { return Mathf.Abs(Get2OrMoreAgentsInRoom() - 1); }, 1f, 0f);
        Factor needToPretend = new LeafVariable(Get2OrMoreAgentsInRoom, 1f, 0f); //Decisive factor
        Factor fear2 = new LeafVariable(GetAgentLeft, 1f, 0f);

        //Graph factors
        Factor fear1 = new LinearCurve(agentsInLastRoom, -1, 1);

        List<Point2D> points = new List<Point2D>();
        points.Add(new Point2D(0, 0));
        points.Add(new Point2D(0.15f, 0));
        points.Add(new Point2D(0.4f, 0.25f));
        points.Add(new Point2D(0.5f, 0.5f));
        points.Add(new Point2D(0.6f, 0.75f));
        points.Add(new Point2D(0.85f, 1));
        points.Add(new Point2D(1, 1));

        Factor riskToLose = new LinearPartsCurve(tasksCompleted, points); //Decisive factor

        //Fusion factors
        List<Factor> factors = new List<Factor>();
        factors.Add(fear1);
        factors.Add(fear2);

        List<float> weights = new List<float>();
        weights.Add(0.5f);
        weights.Add(0.5f);

        Factor fearToBeDiscovered = new WeightedSumFusion(factors, weights);

        factors.Clear();
        factors.Add(riskToLose);
        factors.Add(fearToBeDiscovered);
        factors.Add(killingPossibility);

        weights.Clear();
        weights.Add(0.2f);
        weights.Add(0.3f);
        weights.Add(0.5f);

        Factor killingNeed = new WeightedSumFusion(factors, weights); //Decisive factor

        //Actions and decisions
        killingUS.CreateUtilityAction("sabotear", Sabotage, riskToLose);
        killingUS.CreateUtilityAction("asesinar", Kill, killingNeed);
        UtilityAction pretendUA = killingUS.CreateSubBehaviour("fingir", needToPretend, pretendBT);
        
        //Transition
        BehaviourTreeStatusPerception pretendDone = killingUS.CreatePerception<BehaviourTreeStatusPerception>(pretendBT, ReturnValues.Succeed);
        pretendBT.CreateExitTransition("Exit_Transition", pretendUA.utilityState, pretendDone, killingUS);
    }

    private void CreatePretendBehaviourTree()
    { 
        SequenceNode rootNode = pretendBT.CreateSequenceNode("root", false);

        LeafNode walkToTask = pretendBT.CreateLeafNode("walk to task", WalkToTask, isInObjective);
        LeafNode pretendWork = pretendBT.CreateLeafNode("prtend work", Work, finishedWorking);

        rootNode.AddChild(walkToTask);
        rootNode.AddChild(pretendWork);

        pretendBT.SetRootNode(rootNode);        
    }

    #endregion

    #region EntryUSDataMethods

    //Returns 1 if there's 2 or more honest agents with the traitor
    private float Get2OrMoreAgentsInRoom()
    {
        return 0f;
    }

    private float GetNumberOfAgentsInLastRoom()
    {
        return 0f;
    }

    //Returns 0 if a honest agent just left the room
    private float GetAgentLeft()
    {
        return 0f;
    }

    #endregion

    #region UtilityActionsMethods

    private void Sabotage()
    {
        Debug.Log("He decidido sabotear");
    }

    private void Kill()
    {
        Debug.Log("He decidido matar");
    }

    #endregion

    #region BTActions

    private void WalkToTask()
    {
        agent.SetDestination(currentTask);
    }

    private void Work()
    {
        Debug.Log("Fingiendo");
        StartCoroutine(TimerWork());
    }

    private IEnumerator TimerWork()
    {
        yield return new WaitForSeconds(timeWorking);
    }

    #endregion

    #region BTEvaluationFunctions
    private ReturnValues isInObjective()
    {
        // Checks if agent position is task position
        if (Vector3.Distance(this.transform.position, currentTask) < 3)
        {
            return ReturnValues.Succeed;
        }
        else
        {
            return ReturnValues.Running;
        }
    }

    private ReturnValues finishedWorking()
    {
        return ReturnValues.Succeed;
    }
    #endregion

    #region FSMActions

    private void Wander()
    {
        Debug.Log("Estado Wander");
    }

    #endregion
}
