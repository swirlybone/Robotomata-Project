using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using static NotificationCenter;
using static RayCastingHelper;

//MOVE MORE OF THIS TO THE ROBOT CONTROLLER
//Add states to the boxes to avoid a lot of crappy design

public class Box : MonoBehaviour
{
    int value;
    int labelNumber = 4;
    enum State {INTERACTABLE, NON_INTERACTABLE}
    State state;
    RobotController robot;
    Rigidbody rb;
    BoxCollider bc;

    Ray ray;
    RaycastHit raycastHit;

    // Start is called before the first frame update
    void Start()
    {
        bc = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        rb.mass = Random.Range(.5f, 2f);
        value = Mathf.RoundToInt(10 * rb.mass);
        transform.localScale = transform.localScale * rb.mass;
        NonInteractableMode();
        bc.enabled = true;
    }

    public void Poof()
    {
        PostNotification("AwardMoney", value);
        PostNotification("Bonus", labelNumber);
        Debug.Log("You earn " + value + " money for this succesful shipment");
        Destroy(gameObject);
    }

    public void GrabbedBy(RobotController robot) { this.robot = robot; NonInteractableMode(); }
    public void Drop() { robot = null; InteractableMode(); }
    public void DepositMode() {
        NonInteractableMode();
        if (robot != null)
        {
            robot.LetGo();
            robot = null;
        }
    }

    public void InteractableMode() {
        state = State.INTERACTABLE;
        bc.enabled = true;
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    public void NonInteractableMode() {
        state = State.NON_INTERACTABLE;
        bc.enabled = false;
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    public bool Interactable() => state == State.INTERACTABLE;
    public bool NonInteractable() => state == State.NON_INTERACTABLE;
    public float Mass() => rb.mass;

    public void SetLabel(int num) => labelNumber = num;
}
