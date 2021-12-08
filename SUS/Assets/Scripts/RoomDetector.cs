using System.Collections;
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
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entró en Sala: " + this.name + " el agente: " + other.gameObject.name + " lo mismo: " + other.gameObject.GetComponent<Agent>().getAgentName());
        if (other.gameObject.GetComponent<Agent>() != null)
        {
            newAg = other.gameObject.GetComponent<Agent>();
            setInfoAllAgents(other.gameObject.name, newAg);
            agentsInside.Add(newAg);
        }
        
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Agent>() != null)
            agentsInside.Remove(other.gameObject.GetComponent<Agent>());
            Debug.Log("Salió de Sala: " + this.name + " el agente: " + other.gameObject.name);
    }

    private void setInfoAllAgents(string name,Agent agentInside)
    {
        foreach(Agent ag in agentsInside)
        {
            //ag.agentsInTheRoom.Add(agentInside.getAgentName(), agentInside);
        }
    }
}
