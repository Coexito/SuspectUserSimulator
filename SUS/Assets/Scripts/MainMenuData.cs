using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuData : MonoBehaviour
{
    private int honests;
    private int traitors;
    private int tasks;

    public int Honests { get => honests; set => honests = value; }
    public int Traitors { get => traitors; set => traitors = value; }
    public int Tasks { get => tasks; set => tasks = value; }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

}
