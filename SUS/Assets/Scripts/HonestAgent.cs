using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HonestAgent : Agent
{
    public Dictionary<string, Agent> agentsFoundInKillingRoom;

    public HonestAgent() : base()
    { 
        //SceneController.instance.agents.Add(this.gameObject);
    }

    private void Awake() {
        agentsFoundInKillingRoom = new Dictionary<string, Agent>();
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
