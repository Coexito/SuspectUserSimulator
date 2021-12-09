using System.Collections.Generic;
using UnityEngine;

public class RoomDetector : MonoBehaviour
{
    public List<Agent> agentsInside;
    public List<Agent> corpsesInside;
    public int roomIndex;

    // Start is called before the first frame update
    void Start()
    {
        agentsInside = new List<Agent>();
        corpsesInside =  new List<Agent>();
        //roomIndex = this.gameObject.name();
    }
    
    //When an agent enters into a room
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponent<Agent>() != null)
        {
            other.gameObject.GetComponent<Agent>().SetActualRoom(this);
            //Adding new agent to all inside room agents
            setInfoAllAgents(other.gameObject.GetComponent<Agent>());
            //Adding all agents inside the room to the agent.
            addInsideAgents(other.gameObject.GetComponent<Agent>());
            //Adding new agent to the room list
            agentsInside.Add(other.gameObject.GetComponent<Agent>());
            //Adding the roomIndex
            addRoomsInfo(other.gameObject.GetComponent<Agent>());
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
            UpdateRoomInfo(other.gameObject.GetComponent<Agent>());
        }
    }

    public void AgentKilledInRoom(Agent victim)
    {
        removeInfoAllAgents(victim);
        agentsInside.Remove(victim);
        updateRoomsDictionary(victim);
        corpsesInside.Add(victim);
    }

    public void CorpseFound(Agent corpse)
    {
        corpsesInside.Remove(corpse);
    }


    //Add the incoming agent to all agents inside the room
    private void setInfoAllAgents(Agent agentInside)
    {
        foreach (Agent ag in agentsInside)
        {
            ag.agentsInTheRoom.Add(agentInside.getAgentName(), agentInside);
        }
    }

    //Delete the exit agent from all agents that are inside the room
    private void removeInfoAllAgents(Agent agentInside)
    {
        foreach (Agent ag in agentsInside)
        {
            if (agentInside is HonestAgent)
            {
                if (ag is TraitorAgent)
                    ((TraitorAgent)ag).AgentLeft();
            }

            ag.agentsInTheRoom.Remove(agentInside.getAgentName());
        }
    }

    //Add all agents inside the room to the incoming agent
    private void addInsideAgents(Agent incomingAgent)
    {
        foreach (Agent ag in agentsInside)
            incomingAgent.agentsInTheRoom.Add(ag.getAgentName(), ag);
    }

    //Adding all the info to the last 2 rooms.
    private void updateRoomsDictionary(Agent exitAgent)
    {
        //Clear agentsInThe2Room
        exitAgent.agentsInThe2Room.Clear();
        //Copy everything from agentsInThe1Room to agentsInThe2Room
        foreach (KeyValuePair<string, Agent> ag in exitAgent.agentsInThe1Room)
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

    private void addRoomsInfo(Agent incomingAgent)
    {
        incomingAgent.rooms.Enqueue(roomIndex);
    }
    private void UpdateRoomInfo(Agent exitAgent)
    {
        exitAgent.rooms.Dequeue();
    }
}
