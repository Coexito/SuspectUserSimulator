using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class HonestBehaviour : MonoBehaviour
{
    private StateMachineEngine generalFSM;
    private StateMachineEngine defaultFSM;
    private BehaviourTreeEngine workBT;

    private HonestAgent thisAgent;
    [SerializeField] [Header("Agent speed:")] private float defaultSpeed = 5f;

    private NavMeshAgent agent;
    [SerializeField] private Vector3 currentTask = Vector3.zero;
    private bool taskFound = false;

    [SerializeField] private float timeWorking = 5f;

    void Awake()
    {
        // Creates the object that represents this agent & has data structures
        thisAgent = new HonestAgent(defaultSpeed);

        agent = GetComponent<NavMeshAgent>(); // Gets the navmeshagent
        agent.speed = this.defaultSpeed;

        //Work Behaviour Tree
        workBT = new BehaviourTreeEngine(true);

        // Default submachine for Working
        defaultFSM = new StateMachineEngine(BehaviourEngine.IsNotASubmachine);
        CreateBT();
        CreateFMS();
    }

    void Update()
    {
        workBT.Update();
        defaultFSM.Update();

        //generalFSM.Update();
    }

    private void CreateFMS()
    {

        // Default substates
        State wanderState = defaultFSM.CreateEntryState("wander", Wander);
        State workState = defaultFSM.CreateSubStateMachine("work", workBT);

        // Perceptions
        ValuePerception	task = defaultFSM.CreatePerception<ValuePerception>(() => taskFound == true);
        BehaviourTreeStatusPerception taskDone = defaultFSM.CreatePerception<BehaviourTreeStatusPerception>(workBT, ReturnValues.Succeed);

        // Transitions
        defaultFSM.CreateTransition("task found", wanderState, task, workState);
        workBT.CreateExitTransition("BT done", workState, taskDone, wanderState);


        // //General FSM
        // generalFSM = new StateMachineEngine(BehaviourEngine.IsNotASubmachine);

        // // States
        // State initialState = generalFSM.CreateEntryState("idle");

        //     // Other general states
        // State votingState = generalFSM.CreateState("vote", Vote);
        // State deadState = generalFSM.CreateState("dead", Die);

        // // Perceptions
        // Perception born = generalFSM.CreatePerception<TimerPerception>(3);

        // Perception voteCalled = generalFSM.CreatePerception<PushPerception>();
        // Perception voteFinished = generalFSM.CreatePerception<PushPerception>();
        // Perception youWereKilled = generalFSM.CreatePerception<PushPerception>();


        // Transitions
        //generalFSM.CreateTransition("start working", initialState, born, defaultState); // When born, start working

        // generalFSM.CreateTransition("vote called", defaultState, voteCalled, votingState);
        // generalFSM.CreateTransition("vote finished", votingState, voteCalled, defaultState);
        // generalFSM.CreateTransition("killed", defaultState, youWereKilled, deadState);

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


    #region FSMActions
    private void Wander()
    {
        SceneController.instance.IWantATask(this);
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
    { }

    private void Die()
    { }
    #endregion

    #region BTActions
    private void WalkToTask(Vector3 coords)
    {
        // Resets the bools
        taskFound = false;
        agent.SetDestination(coords);
    }

    private void WorkBT()
    {
        StartCoroutine(TimerWork());
        SceneController.instance.TaskDone();
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
        if(Vector3.Distance(this.transform.position, currentTask) < 3)
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
        currentTask = Vector3.zero;
        return ReturnValues.Succeed;
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

    //Returns 0 if an agent just left the room
    private float GetAgentLeft()
    {
        return 0f;
    }

    #endregion
}
