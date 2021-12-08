using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* La generación aleatoria funciona creando tareas cada x segundos,
    pudiendo repetir el lugar donde crear una tarea.

    Se pasan constantemente tareas nuevas al SceneController (mundo)

    Nunca habrá más de X tareas disponibles a la vez.
*/

public class TaskGenerator : MonoBehaviour
{
    public static TaskGenerator instance;

    public List<Vector3> tasksCoords;    // Lista de posiciones en las que hacer una tarea
    [SerializeField] private float minSecs = 1.0f;        // Minima cantidad de segundos antes de que aparezca el objeto
    [SerializeField] private float maxSecs = 5.0f;        // Maxima cantidad de segundos antes de que aparezca el objeto
    
    [SerializeField] private int MAX_TASKS_AVAILABLE;     // Máximo de tareas activas que puede haber en el mundo
    private bool generating = false;    // para saber si se está generando una tarea en el momento

    void Awake()
    {
        instance = this;
        List<GameObject> tasks = new List<GameObject>();
        tasksCoords = new List<Vector3>();
        SetTasksCoords(tasksCoords);
    }

    private void Start() {
        MAX_TASKS_AVAILABLE = Mathf.RoundToInt(SceneController.instance.GetTotalHonestAgents() / 2) + 1;
        SceneController.instance.EMERGENCY_POINT = transform.Find("EmergencyPoint").transform.position;
    }

    void Update()
    {
        int num = Random.Range(0, tasksCoords.Count); // random task to generate

        // Si no hay objeto ni estamos ya generandolo, lanzamos la rutina de generacion
        if (!generating && SceneController.instance.availableTasks.Count < MAX_TASKS_AVAILABLE)
            StartCoroutine(GenerateTask(num));
    }


    private IEnumerator GenerateTask(int num)
    {
        generating = true;

        // Random seconds interval to generate new task
        yield return new WaitForSeconds(Random.Range(minSecs, maxSecs));

        // Generates new task in the world        
        SceneController.instance.availableTasks.Add(tasksCoords[num]);
        
        generating = false;
    }

    private void SetTasksCoords(List<Vector3> tasks)
    {
        //Add to tasks the GameObjects' Coords
        for (int i = 1; i <= transform.Find("Tasks").transform.childCount; i++)
        {
            tasks.Add(transform.Find("Tasks").transform.Find("Task" + i).transform.localPosition);
        }
    }

}
