using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HonestBehaviour : MonoBehaviour
{
    private StateMachineEngine generalFSM;
    private BehaviourTreeEngine workBT;

    private HonestAgent thisAgent;
    public SceneController worldController;
    [SerializeField] private float defaultSpeed = 5f;

    void Awake()
    {
        // Creates the object that represents this agent & has data structures
        thisAgent = new HonestAgent(defaultSpeed); 


        //Pretend Behaviour Tree
        workBT = new BehaviourTreeEngine(true);

        SequenceNode rootNode = workBT.CreateSequenceNode("root", false);

        LeafNode walkToTask = workBT.CreateLeafNode("walk to task", () => WalkToTask(), () => isInObjective());
        LeafNode work = workBT.CreateLeafNode("work", () => Work(), () => finishedWorking());

        rootNode.AddChild(walkToTask);
        rootNode.AddChild(work);


       //General FSM
        generalFSM = new StateMachineEngine();

        State wanderState = generalFSM.CreateEntryState("wander", () => Wander());
        State workState = generalFSM.CreateEntryState("work",() => Work()); // no estoy seguro de esto?

        TimerPerception cooldownEnded = generalFSM.CreatePerception<TimerPerception>(10);
        PushPerception decisionTaken = generalFSM.CreatePerception<PushPerception>();

        generalFSM.CreateTransition("task found", wanderState, cooldownEnded, workState);
        generalFSM.CreateTransition("task done", workState, decisionTaken, wanderState);
    }

    void Update()
    {
        generalFSM.Update();
        workBT.Update();
    }

    #region FSMActions
    private void Wander()
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
