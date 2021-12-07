using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HonestAgent : Agent
{
    public Dictionary<string, Agent> agentsFoundInKillingRoom;

    public HonestAgent(float _speed) : base(_speed)
    {
        agentsFoundInKillingRoom = new Dictionary<string, Agent>();
        //SceneController.instance.agents.Add(this.gameObject);
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
}
