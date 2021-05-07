using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DepositConveyor : Job
{
    [SerializeField] Transform Node1;
    [SerializeField] Transform Node2;

    IEnumerator MoveBetween(Box box)
    {
        Transform deposit = box.transform;
        //First movement
        while (deposit.position != Node1.position)
        {
            deposit.position = Vector3.MoveTowards(deposit.position, Node1.position, speed); yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        //Second movement
        while (deposit.position != Node2.position)
        {
            deposit.position = Vector3.MoveTowards(deposit.position, Node2.position, speed); yield return new WaitForEndOfFrame();
        }

        box.Poof();
    }

    public override void StartJob(RobotController robot)
    {
        robot.state = RobotController.State.PERFORMING_JOB;
        Debug.Log("Job called successfully");
        Box box = robot.HeldBox();
        robot.LetGo();
        box.DepositMode();
        StartCoroutine(MoveBetween(box));

        robot.state = RobotController.State.WORKING;
        robot.FindNextNode();
    }
}
