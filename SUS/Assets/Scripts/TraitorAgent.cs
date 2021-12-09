using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitorAgent : Agent
{
    public Dictionary<string, Agent> agentsFoundInKillingRoom;
    private bool agentLeft = false;
    private HonestAgent victim;

    public TraitorAgent() : base ()
    {
        agentsFoundInKillingRoom = new Dictionary<string, Agent>();
    }

    private void FixedUpdate()
    {
        if (agentLeft)
            StartCoroutine(TimerAgentLeft());
    }

    private IEnumerator TimerAgentLeft()
    {
        // Stops the agent until he finishes the task   
        yield return new WaitForSeconds(2f);
        agentLeft = false;
    }

    public int NumberOfHonestAgentsInRoom()
    {
        int total = 0;

        foreach (KeyValuePair<string, Agent> ag in agentsInTheRoom)
        {
            if (ag.Value is HonestAgent)
                total++;
        }

        return total;
    }

    public int NumberOfHonestAgentsInLastRoom()
    {
        int total = 0;

        foreach (KeyValuePair<string, Agent> ag in agentsInThe1Room)
        {
            if (ag.Value is HonestAgent)
                total++;
        }

        return total;
    }

    public void AgentLeft()
    {
        agentLeft = true;
    }

    public bool GetAgentLeft()
    {
        return agentLeft;
    }

    public HonestAgent GetVictim()
    {
        return victim;
    }

    public void SetVictim()
    {
        foreach (KeyValuePair<string, Agent> ag in agentsInTheRoom)
        {
            if (ag.Value is HonestAgent)
            {
                victim = (HonestAgent)ag.Value;
                break;
            }
        }
    }

    public override void StartVote()
    {
        base.StartVote();
        TraitorBehaviour behaviour = GetComponent<TraitorBehaviour>();

        if (behaviour)
            behaviour.FireVote();

    }

    public override void FinishVote()
    {
        base.FinishVote();
        TraitorBehaviour behaviour = GetComponent<TraitorBehaviour>();

        if (behaviour)
            behaviour.FireWander();
    }

    public override void StartSabotage(Vector3 sabotagePos)
    {
        base.StartSabotage(sabotagePos);

        TraitorBehaviour behaviour = GetComponent<TraitorBehaviour>();

        if (behaviour)
            behaviour.StartSabotage();
    }

    public override void EndSabotage()
    {
        base.EndSabotage();

        TraitorBehaviour behaviour = GetComponent<TraitorBehaviour>();

        if (behaviour)
            behaviour.EndSabotage();
    }
}