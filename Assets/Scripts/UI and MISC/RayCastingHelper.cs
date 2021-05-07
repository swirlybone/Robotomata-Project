using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastingHelper
{
    static RaycastHit raycastHit;
    static Ray ray;

    private static RayCastingHelper _instance;

    public static RayCastingHelper Instance()
    {
        if (_instance == null)
        {
            _instance = new RayCastingHelper();
        }

        return _instance;
    }
    public static bool CameraRayHit(string tag)
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out raycastHit, Mathf.Infinity);
        return TagHit(tag);
    }

    public static bool TagHit(string tag) => raycastHit.collider.CompareTag(tag);

    public static Ray GetRay() => ray;
    public static RaycastHit GetHit() => raycastHit;
}
