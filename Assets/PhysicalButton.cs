using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhysicalButton : MonoBehaviour
{
    [SerializeField] ApplyLabel job;
    private void OnMouseDown()
    {
        if(!EventSystem.current.IsPointerOverGameObject()) job.ChangeLabelType();
    }
}
