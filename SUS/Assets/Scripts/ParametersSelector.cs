using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ParametersSelector : MonoBehaviour
{
    private const int upLimitHonest = 20;
    private const int downLimitHonest = 2;
    private const int upLimitTraitors = 10;
    private const int downLimitTraitors = 1;
    private const int upLimitTasks = 200;
    private const int downLimitTasks = 10;
    private int honests;
    private int traitors;
    private int tasks;

    GameObject menu;

    TextMeshProUGUI amountHonests;
    TextMeshProUGUI amountTraitors;
    TextMeshProUGUI amountTasks;

    void Start()
    {
        honests = 7;
        traitors = 3;
        tasks = 30;
        menu = GameObject.Find("MainMenuData");
        
        amountHonests = transform.Find("ParametersSelector").transform.Find("HonestAgents").transform.Find("Amount").GetComponent<TextMeshProUGUI>();
        amountTraitors = transform.Find("ParametersSelector/Traitors").transform.Find("Amount").GetComponent<TextMeshProUGUI>();
        amountTasks = transform.Find("ParametersSelector/Tasks").transform.Find("Amount").GetComponent<TextMeshProUGUI>();
    }

    #region ADDERS & REMOVERS

    public void addAmountHonest()
    {
        honests++;
        checkLimitsHonest();
        amountHonests.SetText(honests.ToString());
    }
    public void removeAmountHonest()
    {
        honests--;
        checkLimitsHonest();
        amountHonests.SetText(honests.ToString());
    }
    public void addAmountTraitor()
    {
        traitors++;
        checkLimitsTraitor();
        amountTraitors.SetText(traitors.ToString());
    }
    public void removeAmountTraitor()
    {
        traitors--;
        checkLimitsTraitor();
        amountTraitors.SetText(traitors.ToString());
    }
    public void addAmountTasks()
    {
        tasks+=5;
        checkLimitsTasks();
        amountTasks.SetText(tasks.ToString());
    }
    public void removeAmountTasks()
    {
        tasks-=5;
        checkLimitsTasks();
        amountTasks.SetText(tasks.ToString());
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

    
    public void setMainMenuData()
    {
        menu.GetComponent<MainMenuData>().Honests = honests;
        menu.GetComponent<MainMenuData>().Tasks = tasks;
        menu.GetComponent<MainMenuData>().Traitors = traitors;
    }
    public int Traitors { get => traitors; set => traitors = value; }
    public int Tasks { get => tasks; set => tasks = value; }
    public int Honests { get => honests; set => honests = value; }

}