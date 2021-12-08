using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitorAgent : Agent
{
    public Dictionary<string, Agent> agentsFoundInKillingRoom;

    public TraitorAgent() : base ()
    {
        agentsFoundInKillingRoom = new Dictionary<string, Agent>();
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
    
}