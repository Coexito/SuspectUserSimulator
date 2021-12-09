using System.Collections.Generic;
using UnityEngine;


public class Agent : MonoBehaviour
{
    private float speed; // Speed for the agents
    public Dictionary<string, Agent> agentsInTheRoom;  // Agents in the current room (where this agent is)
    public Dictionary<string, Agent> agentsInThe1Room;   // Agents remembered in the last 2 rooms
    public Dictionary<string, Agent> agentsInThe2Room;
    private string agentName;

    public int[] rooms;
    [SerializeField] private List<string> agentsInTheRoomList;
    [SerializeField] private List<string> agentsInTheRoomList1;
    [SerializeField] private List<string> agentsInTheRoomList2;

    private RoomDetector actualRoom;
    private Vector3 sabotageTask = Vector3.zero;

    public Agent()
    {
        agentsInTheRoom = new Dictionary<string, Agent>();
        agentsInThe1Room = new Dictionary<string, Agent>();
        agentsInThe2Room = new Dictionary<string, Agent>();
        agentsInTheRoomList = new List<string>();
        agentsInTheRoomList2 = new List<string>();
        agentsInTheRoomList1 = new List<string>();
        rooms = new int[3];
    }

    public void SetActualRoom(RoomDetector dt)
    {
        actualRoom = dt;
    }

    public RoomDetector GetActualRoom()
    {
        return actualRoom;
    }

    public void getList()
    {
        foreach (KeyValuePair<string, Agent> ag in agentsInTheRoom)
        {
            agentsInTheRoomList.Add(ag.Key);
        }
        foreach (KeyValuePair<string, Agent> ag in agentsInThe1Room)
        {
            agentsInTheRoomList1.Add(ag.Key);
        }
        foreach (KeyValuePair<string, Agent> ag in agentsInThe2Room)
        {
            agentsInTheRoomList2.Add(ag.Key);
        }
    }
    public void clearList()
    {
        agentsInTheRoomList.Clear();
        agentsInTheRoomList2.Clear();
        agentsInTheRoomList1.Clear();
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
