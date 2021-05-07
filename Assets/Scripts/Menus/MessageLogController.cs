using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using TMPro;
using static NotificationCenter;

public class MessageLogController : MonoBehaviour
{
    public enum State { OPENING, CLOSING, OPEN, CLOSED }
    State state;
    RectTransform me;
    ScrollRect scroll;
    [SerializeField] GameObject contentSpace;
    [SerializeField] GameObject message;

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
        AddObserver("Log", LogMessage);
        AddObserver("Message", LogMessage);
    }

    private void OnDisable()
    {
        RemoveObserver("Log", LogMessage);
        RemoveObserver("Message", LogMessage);
    }

    void Update()
    {
        if (Input.GetKeyDown("space")) ToggleOpen();
    }

    public void ToggleOpen()
    {
        if (Opened()) { StopAllCoroutines(); StartCoroutine(Close()); }
        else { StopAllCoroutines(); StartCoroutine(Open()); }
    }

    IEnumerator Open()
    {
        state = State.OPENING;
        while (me.anchoredPosition.x < xOpen) { me.anchoredPosition += speedFactor; yield return new WaitForEndOfFrame(); }
        yield return new WaitForEndOfFrame();
        state = State.OPEN;
    }

    IEnumerator Close()
    {
        state = State.CLOSING;
        while (me.anchoredPosition.x > xClosed) { me.anchoredPosition -= speedFactor; yield return new WaitForEndOfFrame(); }
        yield return new WaitForEndOfFrame();
        state = State.CLOSED;
    }

    public bool Opened() => state == State.OPEN || state == State.OPENING;

    void LogMessage(Notification notification) { Instantiate(message, contentSpace.transform).GetComponent<TextMeshProUGUI>().text = (string)notification.Object; ScrollToBottom(); }

    public void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }
}