using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitorBehaviour : MonoBehaviour
{
    private UtilitySystemEngine killingUS;
    private BehaviourTreeEngine pretendBT;


    private const float TOTAL_TASKS = 20f; //Number of tasks needed to be done by honest agents
    private const float TOTAL_AGENTS = 5f; //Total number of agents 

    void Awake()
    {
        //Pretend Behaviour Tree

        pretendBT = new BehaviourTreeEngine(true);

        SequenceNode rootNode = pretendBT.CreateSequenceNode("root", false);

        LeafNode walkToTask = pretendBT.CreateLeafNode("walk to task", () => WalkToTask(), () => isInObjective());
        LeafNode pretendWork = pretendBT.CreateLeafNode("prtend work", () => Work(), () => finishedWorking());

        rootNode.AddChild(walkToTask);
        rootNode.AddChild(pretendWork);

        //Killing Utility System

        killingUS = new UtilitySystemEngine(true);

        //Base factors (data received from the world)
        Factor tasksCompleted = new LeafVariable(() => GetNumberOfTasksCompleted(), TOTAL_TASKS, 0f);        
        Factor agentsInLastRoom = new LeafVariable(() => GetNumberOfAgentsInLastRoom(), TOTAL_AGENTS, 0f); 
        
        Factor killingPossibility = new LeafVariable(() => { return Mathf.Abs(Get2OrMoreAgentsInRoom() - 1); }, 1f, 0f);
        Factor needToPretend = new LeafVariable(() => Get2OrMoreAgentsInRoom(), 1f, 0f); //Decisive factor
        Factor fear2 = new LeafVariable(() => GetAgentLeft(), 1f, 0f);

        //Graph factors
        Factor fear1 = new LinearCurve(agentsInLastRoom, -1, 1);
        //Esta deberia ser una curva sigmoidal
        Factor riskToLose = new ExpCurve(tasksCompleted); //Decisive factor

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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        pretendBT.Update();
        killingUS.Update();
    }

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
}
