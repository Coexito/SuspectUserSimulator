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
    [SerializeField]private Vector3 currentTask = Vector3.zero;

    void Awake()
    {
        // Creates the object that represents this agent & has data structures
        thisAgent = new HonestAgent(defaultSpeed); 

        agent = GetComponent<NavMeshAgent>(); // Gets the navmeshagent
        agent.speed = this.defaultSpeed;

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
        // Default submachine for Working
        defaultFSM = new StateMachineEngine(BehaviourEngine.IsNotASubmachine);
        // Default substates
        State wanderState = defaultFSM.CreateEntryState("wander", Wander);
        State waitingState = defaultFSM.CreateState("wait", Wait);
        State workState = defaultFSM.CreateSubStateMachine("work", workBT);

        // Perceptions
        PushPerception taskFound = defaultFSM.CreatePerception<PushPerception>();
        PushPerception taskNotFound = defaultFSM.CreatePerception<PushPerception>();
        TimerPerception waitingForTask = defaultFSM.CreatePerception<TimerPerception>(2);   // Time to wait and search for a task again
        PushPerception taskDone2 = defaultFSM.CreatePerception<PushPerception>();
        BehaviourTreeStatusPerception taskDone = defaultFSM.CreatePerception<BehaviourTreeStatusPerception>(workBT, ReturnValues.Succeed);
        
        // Transitions 
        defaultFSM.CreateTransition("task found", wanderState, taskFound, workState);
        defaultFSM.CreateTransition("not found", wanderState, taskNotFound, waitingState);
        defaultFSM.CreateTransition("wait to task", waitingState, waitingForTask, wanderState);
        workBT.CreateExitTransition("BT done", workState, taskDone, wanderState);
        defaultFSM.CreateTransition("task done", workState, taskDone2, wanderState);  

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
        //Work Behaviour Tree
        workBT = new BehaviourTreeEngine(true);

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
        Debug.Log("Enters in wander");

        foreach (Vector3 task in SceneController.instance.availableTasks)
        {
            // When encountering the first taks, take it and stop searching
            currentTask = task;
            SceneController.instance.TakeTask(task);

            defaultFSM.Fire("task found");
            Debug.Log("Task taken");

            break;
        }

        defaultFSM.Fire("not found");
        
    }

    private void Wait()
    {
        // Caminar aleatoriamente
        Debug.Log("Waiting to find another task");
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
        Debug.Log("Walking to task");
        agent.SetDestination(coords);
    }

    private void WorkBT()
    {
        //workBT.Fire();
    }
    #endregion
    
    #region BTEvaluationFunctions
    private ReturnValues isInObjective()
    {
        // Checks if agent position is task position
        if(Vector3.Distance(this.transform.position, currentTask) < 5)
        {
            Debug.Log("I've arrived");
            return ReturnValues.Succeed;
        } 
        else
        {
            Debug.Log("going...");
            return ReturnValues.Running;
        }
            
        
    }

    private ReturnValues finishedWorking()
    {
        Debug.Log("task done");

        defaultFSM.Fire("task done");
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
