using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static NotificationCenter;

public class SpawnPoint : MonoBehaviour
{
    private void OnMouseDown()
    {
        if(!EventSystem.current.IsPointerOverGameObject()) PostNotification("OpenWallMenu", gameObject);
    }
}
