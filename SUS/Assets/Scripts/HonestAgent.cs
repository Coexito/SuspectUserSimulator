using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HonestAgent : Agent
{
    public Dictionary<string, Agent> agentsFoundInKillingRoom;
    public Dictionary<Agent, int> susValues;
    public int KillRoom;
   
    public HonestAgent() : base()
    {
        agentsFoundInKillingRoom = new Dictionary<string, Agent>();
        susValues = new Dictionary<Agent, int>();
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

    //Adds Suspicious Valous when called
    public void DecideVote(int room)
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i] == room)
            {
               
                switch (i)
                {
                    case 0:
                        foreach (KeyValuePair<string, Agent> ag in agentsInTheRoom)
                        {
                            Debug.Log(getAgentName() + "Sumando valor sala actual" + ag.Value.getAgentName());
                            addSusValuesWhenCloseToTheBody(ag.Value);
                        }
                        break;
                    case 1:
                        foreach (KeyValuePair<string, Agent> ag in agentsInThe1Room)
                        {
                            Debug.Log("Hola");
                            addSusValuesWhenCloseToTheBodyRoom(ag.Value);
                        }
                        break;
                    case 2:
                        foreach (KeyValuePair<string, Agent> ag in agentsInThe2Room)
                        {
                            Debug.Log("Hola");
                            addSusValuesWhenCloseToTheSecondBodyRoom(ag.Value);
                        }
                        break;
                }
            }
        }
    }

    //Adds  Max values when the agent watch the traitors killing
    public void addSusValuesWhenWatchKills(TraitorAgent ag)
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
    public void addSusValuesWhenCloseToTheSecondBodyRoom(Agent ag)
    {
        susValues[ag] += 15;
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
        ShowAgentsList();
        Agent max = new Agent();
        int i = 0;
        foreach (KeyValuePair<Agent, int> ag in susValues)
        {
            if (i == 0)
            {
                max = ag.Key;
                i = 1;
            }
            else
            {
                if (susValues[max] < ag.Value) {
                    max = ag.Key; 
                }
            }
        }
        ReduceSuspiciousValues();
        return GetRandomMostVoted(max);
    }

    //At the end of a votation, we reduce the values of the agents.
    private void ReduceSuspiciousValues()
    {
        foreach(KeyValuePair<Agent, int> ag in susValues)
        {
            susValues[ag.Key] -= 10;
        }
    }
    //If some agents have the same suspicious value, we choose a random agent between them.
    private Agent GetRandomMostVoted(Agent agent)
    {
        List<Agent> sameSusAgents = new List<Agent>();
        sameSusAgents.Add(agent);
        foreach (KeyValuePair<Agent, int> ag in susValues)
        {
            if(susValues[agent] == ag.Value)
            {
                Debug.Log(getAgentName() + ": Tiene el agente: " + agent.getAgentName() + "Una sospecha de: " + susValues[agent] + "Y anade a la lista al agente: " + ag.Key.getAgentName() + "con sospecha: " + ag.Value);
                sameSusAgents.Add(ag.Key);
            }
        }
        int random = Random.Range(0, sameSusAgents.Count);
        return sameSusAgents[random];
    }
    #endregion

    public void ShowAgentsList()
    {
        foreach (KeyValuePair<Agent, int> ag in susValues)
        {
            Debug.Log("AGENTES GUARDADOS: " + ag.Key.getAgentName());
        }
    }
    public override void StartSabotage(Vector3 sabotagePos)
    {
        base.StartSabotage(sabotagePos);

        HonestBehaviour behaviour = GetComponent<HonestBehaviour>();

        if (behaviour)
            behaviour.FireSabotage();
    }

    public override void EndSabotage()
    {
        base.EndSabotage();

        HonestBehaviour behaviour = GetComponent<HonestBehaviour>();

        if (behaviour)
            behaviour.EndSabotage();
    }
}
