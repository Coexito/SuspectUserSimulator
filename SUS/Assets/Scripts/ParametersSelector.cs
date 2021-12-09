using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ParametersSelector : MonoBehaviour
{
    private const int upLimitHonest = 20;
    private const int downLimitHonest = 1;
    private const int upLimitTraitors = 5;
    private const int downLimitTraitors = 0;
    private const int upLimitTasks = 200;
    private const int downLimitTasks = 10;
    private int honests;
    private int traitors;
    private int tasks;

    GameObject menu;

    void Start()
    {
        honests = 7;
        traitors = 3;
        tasks = 30;
        menu = GameObject.Find("MainMenuData");
        setMainMenuData();
    }

    #region ADDERS & REMOVERS

    public void addAmountHonest()
    {
        honests++;
        checkLimitsHonest();
        transform.Find("ParametersSelector").transform.Find("HonestAgents").transform.Find("Amount").GetComponent<TextMeshProUGUI>().SetText(honests.ToString());
        setMainMenuData();
    }
    public void removeAmountHonest()
    {
        honests--;
        checkLimitsHonest();
        transform.Find("ParametersSelector/HonestAgents").transform.Find("Amount").GetComponent<TextMeshProUGUI>().SetText(honests.ToString());
        setMainMenuData();
    }
    public void addAmountTraitor()
    {
        traitors++;
        checkLimitsTraitor();
        transform.Find("ParametersSelector/Traitors").transform.Find("Amount").GetComponent<TextMeshProUGUI>().SetText(traitors.ToString());
    }
    public void removeAmountTraitor()
    {
        traitors--;
        checkLimitsTraitor();
        transform.Find("ParametersSelector/Traitors").transform.Find("Amount").GetComponent<TextMeshProUGUI>().SetText(traitors.ToString());
    }
    public void addAmountTasks()
    {
        tasks+=5;
        checkLimitsTasks();
        transform.Find("ParametersSelector/Tasks").transform.Find("Amount").GetComponent<TextMeshProUGUI>().SetText(tasks.ToString());
    }
    public void removeAmountTasks()
    {
        tasks-=5;
        checkLimitsTasks();
        transform.Find("ParametersSelector/Tasks").transform.Find("Amount").GetComponent<TextMeshProUGUI>().SetText(tasks.ToString());
    }
    #endregion

    #region CHECK LIMITS
    public void checkLimitsHonest()
    {
        if (honests > upLimitHonest)
            honests = upLimitHonest;
        if (honests < downLimitHonest)
            honests = downLimitHonest;
    }
    public void checkLimitsTraitor()
    {
        if (traitors > upLimitTraitors)
            traitors = upLimitTraitors;
        if (traitors < downLimitTraitors)
            traitors = downLimitTraitors;
    }
    public void checkLimitsTasks()
    {
        if (tasks > upLimitTasks)
            tasks = upLimitTasks;
        if (tasks < downLimitTasks)
            tasks = downLimitTasks;
    }
    #endregion

    
    void setMainMenuData()
    {
        menu.GetComponent<MainMenuData>().Honests=honests;
        menu.GetComponent<MainMenuData>().Tasks = tasks;
        menu.GetComponent<MainMenuData>().Traitors = traitors;
    }
    public int Traitors { get => traitors; set => traitors = value; }
    public int Tasks { get => tasks; set => tasks = value; }
    public int Honests { get => honests; set => honests = value; }
}
