using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplyLabel : Job
{
    [SerializeField] Transform enter;
    [SerializeField] Transform labelNode;
    [SerializeField] Transform exit;

    [SerializeField] Transform roboEnter;
    [SerializeField] Transform roboExit;

    [SerializeField] Image image;
    [SerializeField] List<Sprite> images;

    [SerializeField] List<Material> materials;

    Material labelToApply;

    bool Finished = false;

    public override void StartJob(RobotController robot)
    {
        robot.state = RobotController.State.PERFORMING_JOB; //Change to a method
        if (robot.GetType() == typeof(BigRobotController)) StartCoroutine(PerformingBigJob((BigRobotController)robot));
        StartCoroutine(PerformingJob(robot));
    }

    void Start() { labelToApply = materials[0]; image.sprite = images[0]; }

    IEnumerator PerformingJob(RobotController robot)
    {
        yield return robot.MoveTo(roboEnter.position);
        robot.LookAt(enter.position);

        Box box = robot.HeldBox();

        robot.LetGo();
        box.DepositMode();

        while (box.transform.position != enter.position)
        {
            box.transform.position = Vector3.MoveTowards(box.transform.position, enter.position, speed);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();

        while (box.transform.position != labelNode.position)
        {
            box.transform.position = Vector3.MoveTowards(box.transform.position, labelNode.position, speed);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();

        ApplyLabelJob(box);

        yield return robot.MoveTo(roboExit.position);

        robot.LookAt(exit.position);
        while (box.transform.position != exit.position)
        {
            box.transform.position = Vector3.MoveTowards(box.transform.position, exit.position, speed);
            yield return new WaitForEndOfFrame();
        }

        box.InteractableMode();
        robot.Grab(box);
        robot.state = RobotController.State.WORKING;
        robot.FindNextNode();
    }

    IEnumerator PerformingBigJob(BigRobotController robot) 
    {
        yield return robot.MoveTo(roboEnter.position);
        robot.LookAt(enter.position);

        Box box = robot.HeldBox();
        Box otherBox = robot.SecondBox();

        robot.LetGo();
        robot.LetGoOther();

        StartCoroutine(BoxMove(box));
        StartCoroutine(BoxMove(otherBox));

        yield return robot.MoveTo(roboExit.position);

        robot.LookAt(exit.position);

        yield return new WaitUntil(FinishedYet);

        box.InteractableMode();
        otherBox.InteractableMode();

        robot.Grab(box);
        robot.GrabOther(otherBox);

        robot.state = RobotController.State.WORKING;
        robot.FindNextNode();
    }

    IEnumerator BoxMove(Box box) 
    {
        box.DepositMode();

        while (box.transform.position != enter.position)
        {
            box.transform.position = Vector3.MoveTowards(box.transform.position, enter.position, speed);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();

        while (box.transform.position != labelNode.position)
        {
            box.transform.position = Vector3.MoveTowards(box.transform.position, labelNode.position, speed);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();

        ApplyLabelJob(box);

        while (box.transform.position != exit.position)
        {
            box.transform.position = Vector3.MoveTowards(box.transform.position, exit.position, speed);
            yield return new WaitForEndOfFrame();
        }

        Finished = true;
    }

    bool FinishedYet() => Finished;

    void ApplyLabelJob(Box box)
    {
        List<Material> materials = new List<Material>(box.GetComponent<MeshRenderer>().materials);
        materials.Add(labelToApply);
        box.GetComponent<MeshRenderer>().materials = materials.ToArray();
        box.SetLabel(1);
    }

    public void ChangeLabelType() 
    {
        try
        {
            labelToApply = materials[materials.IndexOf(labelToApply) + 1];
            image.sprite = images[materials.IndexOf(labelToApply)];
        }
        catch
        {
            labelToApply = materials[0];
            image.sprite = images[0];
        }
    }
}
