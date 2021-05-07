using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NotificationCenter;

public class RepairRobotController : RobotController
{
    Vector3 origin;

    private void OnEnable()
    {
        AddObserver("ImBroken", GoRepair);
        AddObserver("Placed", FinishPlacement);
    }

    private void OnDisable()
    {
        RemoveObserver("ImBroken", GoRepair);
        RemoveObserver("Placed", FinishPlacement);
    }

    protected new void Update()
    {
        if (PerformingJob()) breakTimer -= Time.deltaTime;
        if (breakTimer <= 0) Break();
    }

    protected new void FixedUpdate() { }

    public override void Initializer()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        state = State.WAITING;
        placed = Spawnable.PlacementState.FLOATING;
        held_state = Holding.EMPTY_HANDED;
    }

    void GoRepair(Notification notification)
    {
        RobotController robot = (RobotController)notification.Object;
        if (!PerformingJob() && !Broken() && placed != PlacementState.FLOATING) {
            robot.SetMechanic(this);
            state = State.PERFORMING_JOB; 
            StopAllCoroutines();
            StartCoroutine(GoRepairBot(robot));
        }
    }

    protected override IEnumerator CreatePath()
    {
        origin = transform.position; 
        gameObject.layer = LayerMask.NameToLayer("Robot");
        PostNotification("DonePlacing"); yield return null;
    }

    IEnumerator GoRepairBot(RobotController robot)
    {
        Vector3 node = robot.repairPoint.transform.position;
        yield return null;
        

        while (Normalize(transform.position) != Normalize(node) && robot.ImMechanic(this))
        {
            Move(node, 1);
            yield return new WaitForFixedUpdate();
        }

        if(robot.ImMechanic(this)) robot.RepairMe(0);
        yield return new WaitForFixedUpdate();
        state = State.WAITING;

        while (Normalize(transform.position) != Normalize(origin))
        {
            Move(origin, 1);
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    new void Break() 
    {
        base.Break();
        StopAllCoroutines();
    }
}
