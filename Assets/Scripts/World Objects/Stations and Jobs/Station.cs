using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Job))]
public class Station : MonoBehaviour
{
    Job job;

    public enum StationState { WORKING, BROKEN, OFF }

    public StationState condition;
    
    [SerializeField] Transform robotNode;
    [SerializeField] int repairCost;

    private void Start()
    {
        condition = StationState.OFF;
        job = GetComponent<Job>();
    }

    private void OnMouseDown()
    {
        if (!Started()) NotificationCenter.PostNotification("StartUp", this);
    }

    public Vector3 TakeJob() 
    {
        Debug.Log(name);
        return robotNode.position;
    }

    public void PerformJob(RobotController robot) 
    {
        job.StartJob(robot);
    }

    public bool Working() => condition == StationState.WORKING;
    public bool Started() => condition != StationState.OFF;
    public bool Broken() => condition == StationState.BROKEN;

    public void Fix() {
        if (Broken() || !Started())
        { 
            condition = StationState.WORKING;
            NotificationCenter.PostNotification("SpendMoney", repairCost);
        }
    }
}