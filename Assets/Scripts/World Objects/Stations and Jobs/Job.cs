using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Job : MonoBehaviour
{
    [SerializeField] protected float speed = .005f;

    float speedMultiplier;



    public abstract void StartJob(RobotController robot);
}
