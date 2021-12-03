using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitorBehaviour : MonoBehaviour
{
    private StateMachineEngine generalFSM;
    private UtilitySystemEngine killingUS;
    private BehaviourTreeEngine pretendBT;

    public SceneController worldController;
    [SerializeField] [Range(0, 20)] [Header("Cooldown time in seconds:")] private int cooldown = 10;

    private void Awake()
    {        
        //Pretend Behaviour Tree
        CreatePretendBehaviourTree();        

        //Killing Utility System
        CreateKillingUtilitySystem();        

        //General FSM
        CreateGeneralFSM();        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pretendBT.Update();
        killingUS.Update();
        generalFSM.Update();
    }

    #region CreateMachines

    private void CreateGeneralFSM()
    {
        generalFSM = new StateMachineEngine();

        State wanderState = generalFSM.CreateEntryState("wander", () => Wander());
        State workState = generalFSM.CreateSubStateMachine("work", killingUS);

        TimerPerception cooldownEnded = generalFSM.CreatePerception<TimerPerception>(cooldown);
        PushPerception decisionTaken = generalFSM.CreatePerception<PushPerception>();

        generalFSM.CreateTransition("cooldown terminado", wanderState, cooldownEnded, workState);
        generalFSM.CreateTransition("decision tomada", workState, decisionTaken, wanderState);
    }

    private void CreateKillingUtilitySystem()
    {
        killingUS = new UtilitySystemEngine(true);

        //Base factors (data received from the world)
        Factor tasksCompleted = new LeafVariable(() => GetNumberOfTasksCompleted(), worldController.GetTotalTasks(), 0f);
        Factor agentsInLastRoom = new LeafVariable(() => GetNumberOfAgentsInLastRoom(), worldController.GetTotalHonestAgents(), 0f);

        Factor killingPossibility = new LeafVariable(() => { return Mathf.Abs(Get2OrMoreAgentsInRoom() - 1); }, 1f, 0f);
        Factor needToPretend = new LeafVariable(() => Get2OrMoreAgentsInRoom(), 1f, 0f); //Decisive factor
        Factor fear2 = new LeafVariable(() => GetAgentLeft(), 1f, 0f);

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
        killingUS.CreateUtilityAction("sabotear", () => Sabotage(), riskToLose);
        killingUS.CreateUtilityAction("asesinar", () => Kill(), killingNeed);
        killingUS.CreateSubBehaviour("fingir", needToPretend, pretendBT);
    }

    private void CreatePretendBehaviourTree()
    {
        pretendBT = new BehaviourTreeEngine(true);

        SequenceNode rootNode = pretendBT.CreateSequenceNode("root", false);

        LeafNode walkToTask = pretendBT.CreateLeafNode("walk to task", () => WalkToTask(), () => isInObjective());
        LeafNode pretendWork = pretendBT.CreateLeafNode("prtend work", () => Work(), () => finishedWorking());

        rootNode.AddChild(walkToTask);
        rootNode.AddChild(pretendWork);
    }

    #endregion

    #region EntryUSDataMethods

    private float GetNumberOfTasksCompleted()
    {
        return 0f;
    }

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
        
    }

    private void Kill()
    {

    }

    private void Pretend()
    {

    }

    #endregion

    #region BTActions

    private void WalkToTask()
    {

    }

    private void Work()
    {

    }

    #endregion

    #region BTEvaluationFunctions
    private ReturnValues isInObjective()
    {
        return ReturnValues.Succeed;
    }

    private ReturnValues finishedWorking()
    {
        return ReturnValues.Succeed;
    }
    #endregion

    #region FSMActions

    private void Wander()
    {

    }

    #endregion
}
