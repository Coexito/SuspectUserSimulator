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
    private const int upLimitTasks = 80;
    private const int downLimitTasks = 10;
    private int honests;
    private int traitors;
    private int tasks;
   
    #region ADDERS & REMOVERS
    
    public void addAmountHonest()
    {
        honests++;
        checkLimitsHonest();
        transform.Find("HonestAgents").transform.Find("Amount").GetComponent<TextMeshProUGUI>().SetText(honests.ToString());
    }
    public void removeAmountHonest()
    {
        honests--;
        checkLimitsHonest();
        transform.Find("HonestAgents").transform.Find("Amount").GetComponent<TextMeshProUGUI>().SetText(honests.ToString());
    }
    public void addAmountTraitor()
    {
        traitors++;
        checkLimitsTraitor();
        transform.Find("Traitors").transform.Find("Amount").GetComponent<TextMeshProUGUI>().SetText(traitors.ToString());
    }
    public void removeAmountTraitor()
    {
        traitors--;
        checkLimitsTraitor();
        transform.Find("Traitors").transform.Find("Amount").GetComponent<TextMeshProUGUI>().SetText(traitors.ToString());
    }
    public void addAmountTasks()
    {
        tasks++;
        checkLimitsTasks();
        transform.Find("Tasks").transform.Find("Amount").GetComponent<TextMeshProUGUI>().SetText(tasks.ToString());
    }
    public void removeAmountTasks()
    {
        tasks--;
        checkLimitsTasks();
        transform.Find("Tasks").transform.Find("Amount").GetComponent<TextMeshProUGUI>().SetText(tasks.ToString());
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

    void Start()
    {
        honests = 5;
        traitors = 2;
        tasks = 10;
    }
}
