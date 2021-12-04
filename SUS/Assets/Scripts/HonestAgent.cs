using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HonestAgent : Agent
{
    public Dictionary<string, Agent> agentsFoundInKillingRoom;

    public HonestAgent(float _speed) : base(_speed)
    {
        agentsFoundInKillingRoom = new Dictionary<string, Agent>();
    }

}
