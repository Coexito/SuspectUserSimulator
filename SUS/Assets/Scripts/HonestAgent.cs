using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HonestAgent : Agent
{
    public Dictionary<string, Agent> agentsFoundInKillingRoom;
    public Dictionary<Agent, int> susValues;
    public HonestAgent() : base()
    { 
        
    }

    private void Awake() {
        agentsFoundInKillingRoom = new Dictionary<string, Agent>();
        susValues = new Dictionary<Agent, int>();
    }

    private void Start() 
    {
        
    }


    public override void StartVote()
    {
        base.StartVote();
        HonestBehaviour behaviour = GetComponent<HonestBehaviour>();

        if(behaviour)
            behaviour.FireVote();

    }

    public override void FinishVote()
    {
        base.FinishVote();
        HonestBehaviour behaviour = GetComponent<HonestBehaviour>();

        if(behaviour)
            behaviour.FireWander();
    }

    public override void Die()
    {
        base.Die();

        HonestBehaviour behaviour = GetComponent<HonestBehaviour>();

        if(behaviour)
            behaviour.FireDie();
    }

    #region SuspiciousFunctions

    //Adds  Max values when the agent watch the traitors killing
    public void addSusValuesWhenWatchKills(Agent ag)
    {
        susValues[ag] = 100;
        CheckMaxAndMinValues(susValues[ag]);
    }
    //Adds sus values when someone is in the body room
    public void addSusValuesWhenCloseToTheBody(Agent ag)
    {
        susValues[ag] += 50;
        CheckMaxAndMinValues(susValues[ag]);
    }

    //Adds sus values when someone is close to the bodyRoom
    public void addSusValuesWhenCloseToTheBodyRoom(Agent ag)
    {
        susValues[ag] += 30;
        CheckMaxAndMinValues(susValues[ag]);
    }

    //Sets the min and max values for sus values
    private void CheckMaxAndMinValues(int val)
    {
        if (val > 100)
            val = 100;
        if (val < 0)
            val = 0;
    }

    //Gets All Agents at the start of the simulation
    public void GetAllAgents(List<GameObject> agents)
    {
        for(int i = 0; i < agents.Count; i++)
        {
            susValues.Add(agents[i].GetComponent<Agent>(),0);
        }
    }

    //Delete dead or expulsed agents from de susValues List.
    public void removeAgent(Agent ag)
    {
        susValues.Remove(ag);
    }

    //Returns the Most Suspicious Agent
    public Agent GetMostSuspiciousAgent()
    {
        Agent max = new Agent();
        int i = 0;
        foreach (KeyValuePair<Agent, int> ag in susValues)
        {
            if (i == 0)
                max = ag.Key;
            else
                if (susValues[max] < ag.Value)
                max = ag.Key;
        }
        return max;
    }
    #endregion
}
