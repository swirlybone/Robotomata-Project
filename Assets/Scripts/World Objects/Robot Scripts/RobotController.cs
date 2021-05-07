using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NotificationCenter;
using static RayCastingHelper;

public class RobotController : Spawnable
{
    public int UpgradeLimit = 5;
    public int upgradedTimes = 0;

    public GameObject boxPosition;
    public GameObject repairPoint;
    public GameObject smoke;

    LineRenderer line;
    Rigidbody rb;
    Box MyBox = null;

    protected List<Station> MyStations;
    protected List<Vector3> JobPositions;
    protected List<Vector3> Nodes;

    RepairRobotController Mechanic;

    #region State Controllers
    public enum State { WORKING, BROKEN, WAITING, PERFORMING_JOB, BEING_REPAIRED }
    public enum Holding { HOLDING, EMPTY_HANDED }

    public State state;
    public Holding held_state;
    #endregion

    protected Vector3 nextPosition;

    public float speed = .05f;
    public float lifespan = 20; //20 Seconds until robot breaks
    [SerializeField] protected float weightTolerance = 1.5f;
    protected float breakTimer;

    #region Monohebaviours
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        line = GetComponent<LineRenderer>();
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        SpawnHeight = (capsule.height / 2f) - capsule.center.y;
        Initializer();

        breakTimer = lifespan;
    }

    private void OnEnable()
    {
        AddObserver("Placed", FinishPlacement);
    }

    private void OnDisable()
    {
        RemoveObserver("Placed", FinishPlacement);
    }

    protected void Update()
    {
        if (HoldingBox()) { MyBox.transform.position = boxPosition.transform.position; }
        if (Working()) breakTimer -= Time.deltaTime;
        if (breakTimer <= 0) { Break(); }
    }

    private void OnMouseOver()
    {
        if (Input.GetKeyDown("t") && !Floating())
        {
            LetGo();
            ReplaceMe();
        }
        else if (Input.GetKeyDown("f") && !Working())
        {
            RepairMe(25);
        }
        else if (Input.GetKeyDown("e")) ShowPath();
    }

    protected void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Box"))
        {
            Box box = other.gameObject.GetComponent<Box>();
            if (!HoldingBox() && Working() && box.Interactable())
            {
                Grab(box);
                if (nextPosition == Nodes[0]) nextPosition = Nodes[1];
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Robot"))
        {
            rb.AddForce(new Vector3(Random.Range(10f, 20f), 0, Random.Range(10f, 20f)));
        }
    }

    protected void FixedUpdate()
    {
        if (Working())
        {
            if (transform.position != nextPosition)
            {
                if (HoldingBox())
                {
                    if (JobPositions.Contains(nextPosition))
                    {
                        if (MyBox.Mass() > weightTolerance) Move(nextPosition, .75f);
                        else Move(nextPosition, 1);
                        FindJobPosition();
                    }
                    else Move(nextPosition, 1);
                }
                else Move(nextPosition, 1);
            }
            else FindNextNode();
        }
    }
    #endregion

    #region Helper Functions
    //Movement related
    public void AddNode(Vector3 node) { Nodes.Add(Normalize(node)); line.positionCount = Nodes.Count; line.SetPositions(Nodes.ToArray()); }
    protected virtual IEnumerator CreatePath()
    {
        gameObject.layer = LayerMask.NameToLayer("Robot");
        AddNode(transform.position);
        while (Waiting())
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (CameraRayHit("Station")) { Debug.Log("Clicked: " + GetHit().collider.name); AddStation(GetHit().collider.GetComponent<Station>()); }
                else if (CameraRayHit("Floor")) AddNode(GetHit().point);
            }

            if (Input.GetMouseButtonDown(1))
            {
                HasEnoughNodes();
            }
            yield return null;
        }
        PostNotification("DonePlacing");
    }
    public void FindNextNode() { try { nextPosition = Nodes[Nodes.IndexOf(nextPosition) + 1]; } catch { nextPosition = Nodes[0]; } }
    public void FinishPlacement(Notification notification)
    {
        if ((GameObject)notification.Object == gameObject)
        {
            Debug.Log("Called finish");
            StartCoroutine(this.CreatePath());
        }
    }
    public void HasEnoughNodes()
    {
        if (Nodes.Count < 2) Debug.LogError("Not enough nodes to finish pathing, select at least 2!"); //Turn into message
        else { state = State.WORKING; nextPosition = Nodes[1]; AddNode(Nodes[0]); line.enabled = false; }
    }
    protected void Move(Vector3 node, float speedMod) { LookAt(node); transform.position = Vector3.MoveTowards(transform.position, Normalize(node), (this.speed * speedMod)); }
    public IEnumerator MoveTo(Vector3 node)
    {
        while (transform.position != Normalize(node)) { Move(Normalize(node), 1); yield return new WaitForFixedUpdate(); }
    }
    public void LookAt(Vector3 node) { transform.LookAt(Normalize(node)); }

    //Job specific
    public void FindJobPosition()
    {
        if (JobPositions.Contains(nextPosition))
            if (Vector3.Distance(transform.position, nextPosition) <= .15f)
                MyStations[JobPositions.IndexOf(nextPosition)].PerformJob(this);
    }


    //Robot specific
    public void Break()
    {
        LetGo();
        state = State.BROKEN; breakTimer = lifespan;
        PostNotification("ImBroken", this);
        Instantiate(smoke, transform.position, transform.rotation).name = "Smoke" + name;
    }
    public virtual void Initializer()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        line.enabled = true;
        state = State.WAITING;
        placed = Spawnable.PlacementState.FLOATING;
        held_state = Holding.EMPTY_HANDED;

        Nodes = new List<Vector3>();
        JobPositions = new List<Vector3>();
        MyStations = new List<Station>();
    }
    public void RepairMe(int amount) { state = State.WORKING; PostNotification("SpendMoney", amount); Destroy(GameObject.Find("Smoke" + name)); }
    public void ReplaceMe() { PostNotification("ReplaceMe", this); }
    public void ShowPath() { if (line.enabled) line.enabled = false; else line.enabled = true; }

    //Upgrades
    public bool CanBeUpgraded() => UpgradeLimit > upgradedTimes;
    public void UpgradeMe() => PostNotification("RobotUpgrade", gameObject);
    public void UpgradeLifeSpan() { upgradedTimes++; lifespan += 5; }
    public void UpgradeSpeed() { upgradedTimes++; speed += .02f; lifespan -= 2.5f; }

    //Box Related
    public void AddStation(Station station)
    {
        MyStations.Add(station);
        JobPositions.Add(Normalize(station.TakeJob()));
        AddNode(station.TakeJob());
    }
    public void Grab(Box box) { box.GrabbedBy(this); MyBox = box; held_state = Holding.HOLDING; }
    public Box HeldBox() => MyBox;
    public void LetGo()
    {
        if (HoldingBox())
        {
            held_state = Holding.EMPTY_HANDED;
            MyBox.Drop();
            MyBox = null;
        }
    }

    //Bools
    public bool HoldingBox() => held_state == Holding.HOLDING;
    public bool Working() => state == State.WORKING;
    public bool Waiting() => state == State.WAITING;
    public bool PerformingJob() => state == State.PERFORMING_JOB;
    public bool Broken() => state == State.BROKEN;

    //RepairBot Helpers
    public void SetMechanic(RepairRobotController robot)
    {
        if (!BeingRepaired()) Mechanic = robot; state = State.BEING_REPAIRED;
    }
    public bool ImMechanic(RepairRobotController robot)
    {
        return robot == Mechanic;
    }
    public bool BeingRepaired() => state == State.BEING_REPAIRED;
    #endregion
}
