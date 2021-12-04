using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskGenerator : MonoBehaviour
{
    /// PARAMETROS QUE PUEDES CONFIGURAR PARA CADA OBJETO DESDE EL INSPECTOR DE UNITY
    public string nombre;                   // Nombre del objeto (debe ser unico)
    public float minSegundos = 1.0f;        // Minima cantidad de segundos antes de que aparezca el objeto
    public float maxSegundos = 5.0f;        // Maxima cantidad de segundos antes de que aparezca el objeto
    public List<Vector3> tasksCoords;    // Maxima distancia del objeto al centro de la escena
    public bool[] tasks;

    // Variables que no se deben tocar
    private bool generando = false;
    private Vector3 posicion = Vector3.zero;
    private bool hayObjeto = false;
    [SerializeField] PlayerController player;
    //*******************************************************

    void Start()
    { 
        tasksCoords = new List<Vector3>();
        tasksCoords.Add(new Vector3(-22.71f, 10.3f, -14.55f));
        tasksCoords.Add(new Vector3(12.1f, 10.3f, -21.64f));
        tasksCoords.Add(new Vector3(39.41f, 10.3f, -4.74f));
        tasksCoords.Add(new Vector3(53.43f, 10.3f, -11.24f));
        tasksCoords.Add(new Vector3(29.2f, 10.3f, 14.3f));
        tasksCoords.Add(new Vector3(18.5f, 10.3f, 3.3f));
        tasksCoords.Add(new Vector3(-13.6f, 10.3f, 19.7f));
        tasks = new bool[tasksCoords.Count];
        // Empezamos sin objeto
        HazInvisible();

        //El Array de tareas lo ponemos en falso al iniciar la partida
        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = false;
        }
    }

    //*******************************************************

    void Update()
    {
        int num = Random.Range(0, tasksCoords.Count);
        // Si no hay objeto ni estamos ya generandolo, lanzamos la rutina de generacion
        if (!generando && !tasks[num])
            StartCoroutine(Generador(num));
    }

    //*******************************************************

    /// <summary>
    /// Rutina de generacion de objeto
    /// </summary>
    /// <returns></returns>
    private IEnumerator Generador(int num)
    {
        generando = true;

        // Espera un tiempo aleatorio para generar un nuevo objeto
        yield return new WaitForSeconds(Random.Range(minSegundos, maxSegundos));

        // Genera nuevo objeto pasado ese tiempo
        tasks[num] = true;
        HazVisible(tasksCoords[num]);
      //  posicion = tasksCoords[3];
       // transform.position = posicion;
        generando = false;

        // Notifica generacion al controlador del mundo
        //if (mundo != null)
        //    mundo.ObjetoGenerado(nombre, posicion);
    }

    //*******************************************************

    /// <summary>
    ///  Hace invisible el objeto
    ///  Ante las muchas posibilidades de implementacion, hemos optado por la menos dependiente de version de Unity
    /// </summary>    
    public void HazInvisible()
    {
       // transform.position = new Vector3(transform.position.x, -10.0f, transform.position.z);
    }

    //*******************************************************

    /// <summary>
    ///  Hace visible el objeto
    ///  Ante las muchas posibilidades de implementacion, hemos optado por la menos dependiente de version de Unity
    /// </summary>    
    public void HazVisible(Vector3 coords)
    {
        player.SetTask(coords);
      //  transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
    }

    //*******************************************************

    /// <summary>
    /// Indica si hay un objeto generado
    /// </summary>
    /// <returns></returns>
    public bool HayObjeto()
    {
        return hayObjeto;
    }

    //*******************************************************

    /// <summary>
    ///  Devuelve la posicion actual del objeto
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPosicion()
    {
        return posicion;
    }

    //*******************************************************

    /// <summary>
    ///  Consume el objeto (lo hace invisible y reanuda el proceso de generacion)
    /// </summary>
    public void Consume(int num)
    {
        tasks[num] = false;
        HazInvisible();
    }
}
