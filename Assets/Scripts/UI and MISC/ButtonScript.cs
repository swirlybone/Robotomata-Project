using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NotificationCenter;

public class ButtonScript : MonoBehaviour
{
    GameObject thing;

    [SerializeField] TMP_Text text;

    public void SetMe(GameObject dodad) { thing = dodad; text.text = dodad.name; }

    public void DoThing() { 
        PostNotification("Zoom", thing);
    }
}
