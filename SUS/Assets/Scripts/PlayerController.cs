using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent agent;
    // Update is called once per frame
    void Update()
    {

    }
    public void SetTask(Vector3 coords)
    {
        agent.SetDestination(coords);
    }
}
