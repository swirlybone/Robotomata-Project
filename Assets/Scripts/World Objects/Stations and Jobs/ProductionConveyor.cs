using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NotificationCenter;
using UnityEngine.EventSystems;

public class ProductionConveyor : Job
{
    float breakTimer = 20;
    public float lifespan = 200;
    // Start is called before the first frame update
    [SerializeField] GameObject package;
    [SerializeField] bool stopSpawning = true;
    [SerializeField] float spawnTime;
    [SerializeField] public float spawnDelay;
    [SerializeField] Transform Node1;
    [SerializeField] Transform Node2;

    void Start() => InvokeRepeating(nameof(SpawnObject), spawnTime, spawnDelay);

    private void OnEnable()
    {
        AddObserver("Open", Unlock);
    }

    private void OnDisable()
    {
        RemoveObserver("Open", Unlock);
    }

    void Unlock(Notification notification) 
    {
        if ((ProductionConveyor)notification.Object == this) stopSpawning = false;
    }

    public void SpawnObject()
    {
        if (!stopSpawning)
        {
            Box box = Instantiate(package, Node1.position, Quaternion.identity).GetComponent<Box>();
            StartCoroutine(MoveBetween(box));
        }
    }

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
        yield return new WaitForEndOfFrame();
        box.InteractableMode();
    }

    public override void StartJob(RobotController robot)
    {

    }
}
