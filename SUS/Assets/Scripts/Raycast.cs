using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    // Start is called before the first frame update
    Ray thugRay;
    public Color rayColor;
    RaycastHit rayHit;
    bool follow;
    void Start()
    {
       
    }
    void Update()
    {
        RaycastHit hit;
        thugRay = new Ray(transform.position, Vector3.down * 1000);
        Debug.DrawRay(transform.position, Vector3.down * 1000, rayColor);//helps me see the ray
        if (Physics.Raycast(thugRay, out hit, 200f))
        {
            Debug.Log("Pasó por la puerta");
        }
    }
}