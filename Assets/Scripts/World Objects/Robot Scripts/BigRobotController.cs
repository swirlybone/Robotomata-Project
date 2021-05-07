using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigRobotController : RobotController
{
    [SerializeField] GameObject boxPosition2;
    Box MyOtherBox;

    private new void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Box"))
        {
            Box box = other.gameObject.GetComponent<Box>();
            if (!HoldingBox() && Working() && box.Interactable())
            {
                Grab(box);
            }
            else if (HoldingBox() && SecondBox() == null)
            {
                if (box != HeldBox()) GrabOther(box);
            }
        }
    }

    protected new void Update()
    {
        if (HoldingBox()) HeldBox().transform.position = boxPosition.transform.position;
        if (SecondBox() != null) SecondBox().transform.position = boxPosition2.transform.position;
        if (Working()) breakTimer -= Time.deltaTime;
        if (breakTimer <= 0) { Break(); }
    }

    protected new void FixedUpdate()
    {
        if (Working())
        {
            if (transform.position != nextPosition)
            {
                if (SecondBox() != null)
                {
                    if ((HeldBox().Mass() + SecondBox().Mass()) > weightTolerance) { Move(nextPosition, .75f); }
                    else Move(nextPosition, 1);
                    FindJobPosition();
                }
                else if (HoldingBox())
                {
                    if (HeldBox().Mass() > weightTolerance) Move(nextPosition, .75f);
                    else Move(nextPosition, 1);
                    FindJobPosition();
                }
                else Move(nextPosition, 1);
            }
            else FindNextNode();
        }
    }

    public void GrabOther(Box box) { box.GrabbedBy(this); MyOtherBox = box; }
    public Box SecondBox() => MyOtherBox;
    public void LetGoOther() {
        if (SecondBox() != null)
        {
            MyOtherBox.Drop();
            MyOtherBox = null;
        }
    }
}
