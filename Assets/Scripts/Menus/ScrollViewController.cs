using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using static NotificationCenter;

public class ScrollViewController : MonoBehaviour
{
    public enum State { OPENING, CLOSING, OPEN, CLOSED }
    State state;
    RectTransform me;
    ScrollRect scroll;
    [SerializeField] GameObject contentSpace;
    [SerializeField] GameObject buttonTemplate;

    float xOpen;
    float xClosed;
    [SerializeField] float speed;
    Vector2 speedFactor;

    void Start()
    {
        me = GetComponent<RectTransform>();
        scroll = GetComponent<ScrollRect>();

        xOpen = me.anchoredPosition.x;
        xClosed = -xOpen;
        StartCoroutine(Close());
        speedFactor = new Vector2(speed, 0f);
    }

    private void OnEnable()
    {
        AddObserver("Added", Added);
    }

    private void OnDisable()
    {
        RemoveObserver("Added", Added);
    }

    void Update()
    {
        if (Input.GetKeyDown("tab")) ToggleOpen();
    }

    public void ToggleOpen() 
    {
        if (Opened()) { StopAllCoroutines(); StartCoroutine(Close()); PostNotification("UnZoom"); }
        else { StopAllCoroutines(); StartCoroutine(Open()); }
    }

    IEnumerator Open() 
    {
        state = State.OPENING;
        while (me.anchoredPosition.x > xOpen) { me.anchoredPosition -= speedFactor; yield return new WaitForEndOfFrame(); }
        yield return new WaitForEndOfFrame();
        state = State.OPEN;
    }

    IEnumerator Close()
    {
        state = State.CLOSING;
        while (me.anchoredPosition.x < xClosed) { me.anchoredPosition += speedFactor; yield return new WaitForEndOfFrame(); }
        yield return new WaitForEndOfFrame();
        state = State.CLOSED;
    }

    public bool Opened() => state == State.OPEN || state == State.OPENING;

    void Added(Notification notification) 
    {
        ButtonScript button = Instantiate(buttonTemplate, contentSpace.transform).GetComponent<ButtonScript>();
        button.SetMe((GameObject)notification.Object);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ScrollViewController))]
public class ScrollTester : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Test Notification")) PostNotification("Added", new GameObject());
    }
}
#endif
