using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent
{
    private float speed; // Speed for the agents
    public Dictionary<string, Agent> agentsInTheRoom;  // Agents in the current room (where this agent is)
    public Dictionary<string, Agent> agentsInMemory;   // Agents remembered in the last 2 rooms
    public Dictionary<string, Agent> sussyAgents;      // Agents suspected (to vote them later)
    public List<Vector3> objectives;                   // Position of objectives in the map

    public Agent(float _speed)
    {
        agentsInTheRoom = new Dictionary<string, Agent>();
        agentsInMemory = new Dictionary<string, Agent>();
        sussyAgents = new Dictionary<string, Agent>();
        objectives = new List<Vector3>();

        speed = _speed;
    }
    
    public void setSpeed(float s)
    {
        speed = s;
    }
    public float getSpeed()
    {
        return this.speed;
    }
    

}
