using System.Collections.Generic;
using UnityEngine;
 

public class Agent : MonoBehaviour
{
    private float speed; // Speed for the agents
    public Dictionary<string, Agent> agentsInTheRoom;  // Agents in the current room (where this agent is)
    public Dictionary<string, Agent> agentsInMemory;   // Agents remembered in the last 2 rooms
    public Dictionary<string, Agent> sussyAgents;      // Agents suspected (to vote them later)
    private string agentName;

    public Agent(float _speed)
    {  
        agentsInTheRoom = new Dictionary<string, Agent>();
        agentsInMemory = new Dictionary<string, Agent>();
        sussyAgents = new Dictionary<string, Agent>();

        agentName = "Agent" + Random.Range(0, 10000);

        this.speed = _speed;
    }

    private void Awake() {  }
    
    public void setSpeed(float s)
    {
        speed = s;
    }
    public float getSpeed()
    {
        return this.speed;
    }

    public string getAgentName() {
        return this.agentName;
    }

    public void setAgentName(string s) {
        this.agentName = s;
    }
    
    public virtual void StartVote() { }

    public virtual void FinishVote() { }

    public virtual void Die() { }
}
