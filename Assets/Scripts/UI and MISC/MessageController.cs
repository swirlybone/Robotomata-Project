using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using TMPro;
using static NotificationCenter;

public class MessageController : MonoBehaviour
{
    public enum State { OPENING, CLOSING, OPEN, CLOSED }
    State state;

    [SerializeField] TextMeshProUGUI message;
    [SerializeField] TextMeshProUGUI buttonText;

    RectTransform me;

    float xOpen;
    float xClosed;
    [SerializeField] float speed;
    Vector2 speedFactor;

    // Start is called before the first frame update
    void Start()
    {
        me = GetComponent<RectTransform>();

        message.text = "Messages appear here!";
        buttonText.text = "Open";

        xOpen = me.anchoredPosition.y;

        xClosed = xOpen - 320;
        StartCoroutine(Close());
        speedFactor = new Vector2(0, speed);
    }

    private void OnEnable()
    {
        AddObserver("Message", NewMessage);
    }

    private void OnDisable()
    {
        RemoveObserver("Message", NewMessage);
    }

    void NewMessage(Notification notification) { StartCoroutine(PostMessage((string)notification.Object)); }

    IEnumerator PostMessage(string text)
    {
        if (Opened()) { yield return Close(); }
        yield return new WaitForSeconds(1);
        message.text = text;
        yield return Open();
        yield return new WaitForEndOfFrame();
    }

    IEnumerator Open()
    {
        buttonText.text = "Close";
        state = State.OPENING;
        while (me.anchoredPosition.y < xOpen) { me.anchoredPosition += speedFactor; yield return new WaitForEndOfFrame(); }
        yield return new WaitForEndOfFrame();
        state = State.OPEN;
    }

    IEnumerator Close()
    {
        buttonText.text = "Open";
        state = State.CLOSING;
        while (me.anchoredPosition.y > xClosed) { me.anchoredPosition -= speedFactor; yield return new WaitForEndOfFrame(); }
        yield return new WaitForEndOfFrame();
        state = State.CLOSED;
    }

    public void ToggleOpen()
    {
        if (Opened()) { StopAllCoroutines(); StartCoroutine(Close()); }
        else { StopAllCoroutines(); StartCoroutine(Open()); }
    }

    public bool Opened() => state == State.OPEN || state == State.OPENING;
}