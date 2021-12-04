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
    [SerializeField] private List<Vector3> tasksCoords;    // Lista de posiciones en las que hacer una tarea
    [SerializeField] private float minSecs = 1.0f;        // Minima cantidad de segundos antes de que aparezca el objeto
    [SerializeField] private float maxSecs = 5.0f;        // Maxima cantidad de segundos antes de que aparezca el objeto
    
    private const int MAX_TASKS_AVAILABLE = 3;            // Máximo de tareas activas que puede haber en el mundo
    private bool generating = false;    // para saber si se está generando una tarea en el momento


    void Awake()
    {
        tasksCoords = new List<Vector3>();
        SetTasksCoords(tasksCoords);
    }

    void Update()
    {
        int num = Random.Range(0, tasksCoords.Count); // random task to generate

        // Si no hay objeto ni estamos ya generandolo, lanzamos la rutina de generacion
        if (!generating && SceneController.instance.availableTaks.Count < MAX_TASKS_AVAILABLE)
            StartCoroutine(GenerateTask(num));
    }


    private IEnumerator GenerateTask(int num)
    {
        generating = true;

        // Random seconds interval to generate new task
        yield return new WaitForSeconds(Random.Range(minSecs, maxSecs));

        // Generates new task in the world        
        SceneController.instance.availableTaks.Add(tasksCoords[num]);
        
        generating = false;
    }

    private void SetTasksCoords(List<Vector3> tasks)
    {
        tasks.Add(new Vector3(-22.71f, 10.3f, -14.55f));
        tasks.Add(new Vector3(12.1f, 10.3f, -21.64f));
        tasks.Add(new Vector3(39.41f, 10.3f, -4.74f));
        tasks.Add(new Vector3(53.43f, 10.3f, -11.24f));
        tasks.Add(new Vector3(29.2f, 10.3f, 14.3f));
        tasks.Add(new Vector3(18.5f, 10.3f, 3.3f));
        tasks.Add(new Vector3(-13.6f, 10.3f, 19.7f));
    }

}
