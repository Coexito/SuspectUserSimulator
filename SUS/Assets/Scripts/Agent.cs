using System.Collections.Generic;
using UnityEngine;


public class Agent : MonoBehaviour
{
    private float speed; // Speed for the agents
    public Dictionary<string, Agent> agentsInTheRoom;  // Agents in the current room (where this agent is)
    public Dictionary<string, Agent> agentsInThe1Room;   // Agents remembered in the last 2 rooms
    public Dictionary<string, Agent> agentsInThe2Room;
    private string agentName;

    public Queue<int> rooms;
    private RoomDetector actualRoom;
    private Vector3 sabotageTask = Vector3.zero;

    public Agent()
    {
        agentsInTheRoom = new Dictionary<string, Agent>();
        agentsInThe1Room = new Dictionary<string, Agent>();
        agentsInThe2Room = new Dictionary<string, Agent>();
        rooms = new Queue<int>();
    }
    private void roomInfo(int room1, int numSala)
    {
        if (agentName == "Agent1")
            Debug.Log(agentName + "Sala num: " + numSala + ": tiene las salas: " + room1);
    }

    public void SetActualRoom(RoomDetector dt)
    {
        actualRoom = dt;
    }

    public RoomDetector GetActualRoom()
    {
        return actualRoom;
    }

    public void setSpeed(float s)
    {
        speed = s;
    }
    public float getSpeed()
    {
        return this.speed;
    }

    public string getAgentName()
    {
        return this.agentName;
    }

    public void setAgentName(string s)
    {
        this.agentName = s;
    }

    public virtual void StartVote() { }

    public virtual void FinishVote() { }

    public virtual void Die() { }

    public virtual void StartSabotage(Vector3 sabotagePos) { sabotageTask = sabotagePos; }

    public virtual void EndSabotage() { }

    public Vector3 GetSabotagePoint()
    {
        return sabotageTask;
    }

}
