using UnityEngine;

public class AlwaysLookAtCamera : MonoBehaviour {

    #region variables

    [SerializeField] private GameObject target;

    #endregion variables

    private void Start() {
        target = GameObject.Find("RTS_Camera").gameObject;
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 p = target.transform.position;
        transform.LookAt(2*transform.position - p); // So it's not facing the back
    }
}