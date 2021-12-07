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
        defaultFSM = new StateMachineEngine(BehaviourEngine.IsASubmachine);
        //General FSM
        generalFSM = new StateMachineEngine(BehaviourEngine.IsNotASubmachine);

        CreateBT();
        CreateFMS();
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

        // Transitions
        defaultFSM.CreateTransition("task found", wanderState, task, workState);
        workBT.CreateExitTransition("BT done", workState, taskDone, wanderState);

        
        // States
        State initialState = generalFSM.CreateEntryState("idle");
        State defaultState = generalFSM.CreateSubStateMachine("default", defaultFSM);
        State votingState = generalFSM.CreateState("vote", Vote);
        State deadState = generalFSM.CreateState("dead", Die);

        // Perceptions
        Perception born = generalFSM.CreatePerception<TimerPerception>(1f);
        Perception voteCalled = generalFSM.CreatePerception<PushPerception>();
        Perception voteFinished = generalFSM.CreatePerception<PushPerception>();
        Perception youWereKilled = generalFSM.CreatePerception<PushPerception>();


        // Transitions
        generalFSM.CreateTransition("born", initialState, born, defaultState); // When born, enters the default state & starts looking for work
        defaultFSM.CreateExitTransition("vote called from work", workState, voteCalled, votingState);
        defaultFSM.CreateExitTransition("vote called from wander", wanderState, voteCalled, votingState);
        generalFSM.CreateTransition("vote finished", votingState, voteCalled, initialState);
        defaultFSM.CreateExitTransition("killed from work", workState, youWereKilled, deadState);
        defaultFSM.CreateExitTransition("killed from wander", wanderState, youWereKilled, deadState);

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
        Debug.Log("Entra en wander");
        this.GetComponentInParent<Renderer>().material.SetColor("_Color", Color.blue);
        SceneController.instance.IWantATask(this);  // Asks for a task

        agent.speed = thisAgent.getSpeed(); // Sets the default speed
        agent.SetDestination(GetRandomPoint(transform.position, 20f));  // Walks randomly until given a task
        
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
        Debug.Log("Juanjo for president");

        // Dismisses his task
        taskFound = false;  
        currentTask = Vector3.zero;

        agent.SetDestination(transform.position);
        this.GetComponentInParent<Renderer>().material.SetColor("_Color", Color.white);
    }

    public void FireVote()
    {
        defaultFSM.Fire("vote called from work");
        defaultFSM.Fire("vote called from wander");
    }

    public void FireWander()
    {
        generalFSM.Fire("vote finished");
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
        this.GetComponentInParent<CapsuleCollider>().enabled = false;
        agent.SetDestination(transform.position);
        this.GetComponentInParent<Renderer>().material.SetColor("_Color", Color.black);
        //Destroy(this.gameObject);
    }

    public void FireDie()
    {
        defaultFSM.Fire("killed from work");
        defaultFSM.Fire("killed from wander");
    }
    #endregion

    #region BTActions
    private void WalkToTask(Vector3 coords)
    {
        // Resets the bools
        Debug.Log("going...");
        taskFound = false;
        agent.SetDestination(coords);
    }

    private void WorkBT()
    {
        Debug.Log("Entra en WorkBT");
        StartCoroutine(TimerWork());
        SceneController.instance.TaskDone();
    }

    private IEnumerator TimerWork()
    {
        // Stops the agent until he finishes the task
        agent.speed = 0;
        yield return new WaitForSeconds(timeWorking);
        agent.speed = thisAgent.getSpeed();
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
