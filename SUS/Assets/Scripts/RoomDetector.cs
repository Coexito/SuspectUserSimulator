using System.Collections.Generic;
using UnityEngine;

public class RoomDetector : MonoBehaviour
{
    public List<Agent> agentsInside;
    Agent newAg;
    // Start is called before the first frame update
    void Start()
    {
        agentsInside = new List<Agent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //When an agent enters into a room
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Agent>() != null)
        { 
            //Adding new agent to all inside room agents
            setInfoAllAgents(other.gameObject.GetComponent<Agent>());
            //Adding all agents inside the room to the agent.
            addInsideAgents(other.gameObject.GetComponent<Agent>());
            //Adding new agent to the room list
            agentsInside.Add(other.gameObject.GetComponent<Agent>());            
        }
        //BORRAR CUANDO COMPROBEMOS QUE FUNCIONA TODO NICE :D
        other.gameObject.GetComponent<Agent>().clearList();
        other.gameObject.GetComponent<Agent>().getList();
        foreach (Agent ag in agentsInside)
        { 
            ag.clearList();
            ag.getList();
        }
    }

    //When an agent exits a room
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Agent>() != null)
        {
            removeInfoAllAgents(other.gameObject.GetComponent<Agent>());
            agentsInside.Remove(other.gameObject.GetComponent<Agent>());
            updateRoomsDictionary(other.gameObject.GetComponent<Agent>());
        }
        //BORRAR CUANDO COMPROBEMOS QUE FUNCIONA TODO NICE :D
        other.gameObject.GetComponent<Agent>().clearList();
        other.gameObject.GetComponent<Agent>().getList();
        foreach (Agent ag in agentsInside)
        {
            ag.clearList();
            ag.getList();
        }
    }


    //Add the incoming agent to all agents inside the room
    private void setInfoAllAgents(Agent agentInside)
    {
        foreach(Agent ag in agentsInside)
        {
            ag.agentsInTheRoom.Add(agentInside.getAgentName(), agentInside);
        }
    }

    //Delete the exit agent from all agents that are inside the room
    private void removeInfoAllAgents(Agent agentInside)
    {
        foreach(Agent ag in agentsInside)
        {
            if (agentInside is HonestAgent)
            {
                if (ag is TraitorAgent)
                    ((TraitorAgent)ag).AgentLeft();
            }

            ag.agentsInTheRoom.Remove(agentInside.getAgentName());
            agentInside.clearList();
            agentInside.getList();
            ag.clearList();
            ag.getList();
        }
    }

    //Add all agents inside the room to the incoming agent
    private void addInsideAgents(Agent incomingAgent)
    {
        foreach(Agent ag in agentsInside)
        {
            incomingAgent.agentsInTheRoom.Add(ag.getAgentName(), ag);
        }
    }

    //Adding all the info to the last 2 rooms.
    private void updateRoomsDictionary(Agent exitAgent)
    {
        //Clear agentsInThe2Room
        exitAgent.agentsInThe2Room.Clear();
        //Copy everything from agentsInThe1Room to agentsInThe2Room
        foreach (KeyValuePair<string,Agent> ag in exitAgent.agentsInThe1Room)
        {
            exitAgent.agentsInThe2Room.Add(ag.Key, (Agent)ag.Value);
        }
        //Clear agentsInThe1Room
        exitAgent.agentsInThe1Room.Clear();
        //Copy everything from agentsInTheRoom to agentsInThe1Room
        foreach (KeyValuePair<string, Agent> ag in exitAgent.agentsInTheRoom)
        {
            exitAgent.agentsInThe1Room.Add(ag.Key, (Agent)ag.Value);
        }
        //Clear agentsInTheRoom
        exitAgent.agentsInTheRoom.Clear();
    }

}
