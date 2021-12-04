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
    private Vector3 currentTask = Vector3.zero;

    void Awake()
    {
        // Creates the object that represents this agent & has data structures
        thisAgent = new HonestAgent(defaultSpeed); 

        agent = GetComponent<NavMeshAgent>(); // Gets the navmeshagent

        CreateFMS();
        CreateBT();
    }

    void Update()
    {
        generalFSM.Update();
        defaultFSM.Update();
        workBT.Update();
    }

    private void CreateFMS()
    {
        //General FSM
        generalFSM = new StateMachineEngine(BehaviourEngine.IsNotASubmachine);
        defaultFSM = new StateMachineEngine(BehaviourEngine.IsASubmachine);


        // States
        State initialState = generalFSM.CreateEntryState("idle");
            // Default substates
        State defaultState = generalFSM.CreateSubStateMachine("default", defaultFSM);
        State wanderState = defaultFSM.CreateEntryState("wander", () => Wander());
        State workState = defaultFSM.CreateState("work", Work);
            // Other general states
        State votingState = generalFSM.CreateState("vote", Vote);
        State deadState = generalFSM.CreateState("dead", Die);


        // Perceptions
        Perception born = generalFSM.CreatePerception<TimerPerception>(3);

        Perception taskFound = defaultFSM.CreatePerception<TimerPerception>(10);
        Perception taskDone = defaultFSM.CreatePerception<PushPerception>();
        Perception arrivedToTask = generalFSM.CreatePerception<ArriveToDestination>(new ArriveToDestination());
        Perception voteCalled = generalFSM.CreatePerception<PushPerception>();
        Perception voteFinished = generalFSM.CreatePerception<PushPerception>();
        Perception youWereKilled = generalFSM.CreatePerception<PushPerception>();


        // Transitions
        generalFSM.CreateTransition("start working", initialState, born, defaultState); // When born, start working
        defaultFSM.CreateTransition("task found", wanderState, taskFound, workState);
        defaultFSM.CreateTransition("task done", workState, taskDone, wanderState);
        generalFSM.CreateTransition("vote called", defaultState, voteCalled, votingState);
        generalFSM.CreateTransition("vote finished", votingState, voteCalled, defaultState);
        generalFSM.CreateTransition("killed", defaultState, youWereKilled, deadState);

    }
    private void CreateBT()
    {
        //Work Behaviour Tree
        workBT = new BehaviourTreeEngine(true);

        SequenceNode rootNode = workBT.CreateSequenceNode("root", false);

        LeafNode walkToTask = workBT.CreateLeafNode("walk to task", () => WalkToTask(currentTask), () => isInObjective());
        LeafNode work = workBT.CreateLeafNode("work", () => Work(), () => finishedWorking());

        rootNode.AddChild(walkToTask);
        rootNode.AddChild(work);
    }


    #region FSMActions
    private void Wander()
    {
        foreach (Vector3 task in SceneController.instance.availableTaks)
        {
            // When encountering the first taks, take it and stop searching
            currentTask = task;
            SceneController.instance.TakeTask(task);

            //////////// Problema con el BT (leaf walkToTask)
            defaultFSM.Fire("task found");
            //WalkToTask(currentTask);
            ////////////

            break;
        }
    }

    private void Work()
    {
        //workBT.Fire();
    }

    private void Vote()
    {
        
    }

    private void Die()
    {
        
    }
    #endregion

    #region BTActions
    private void WalkToTask(Vector3 coords)
    {
        agent.SetDestination(coords);
    }
    #endregion
    
    #region BTEvaluationFunctions
    private ReturnValues isInObjective()
    {
        return ReturnValues.Succeed;
    }

    private ReturnValues finishedWorking()
    {
        generalFSM.Fire("task done");
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
