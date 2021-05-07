using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static NotificationCenter;

public class DestructibleWall : MonoBehaviour
{
    [SerializeField] int cost;
    [SerializeField] float speed;
    [SerializeField] Canvas canvas;
    [SerializeField] Light pointLight;
    [SerializeField] Light spotLight;

    private void OnEnable()
    {
        AddObserver("Sell", Sell);
    }

    private void OnDisable()
    {
        RemoveObserver("Sell", Sell);
    }

    private void Start()
    {
        canvas.enabled = false;
    }

    public int Cost { get => cost; }

    protected virtual void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject()) ToggleMenu();
    }

    public void SellMe() => PostNotification("TrySellThis", this);
    public void DontSell() => canvas.enabled = false;

    void ToggleMenu() { if (canvas.enabled) canvas.enabled = false; else canvas.enabled = true; }

    protected virtual void Sell(Notification notification)
    {
        if ((DestructibleWall)notification.Object == this)
        {
            StartCoroutine(WallSlide());
        }
    }

    IEnumerator WallSlide()
    {
        float s = 0;
        while (s < 30)
        {
            s += speed;
            transform.position -= new Vector3(0, speed, 0); yield return new WaitForFixedUpdate();
        }
        yield return new WaitForEndOfFrame();
        yield return Flicker();
        Destroy(gameObject);
    }

    IEnumerator Flicker()
    {
        pointLight.enabled = true;
        spotLight.enabled = true;
        yield return new WaitForSeconds(.1f);
        pointLight.enabled = false;
        spotLight.enabled = false;
        yield return new WaitForSeconds(.2f);
        pointLight.enabled = true;
        spotLight.enabled = true;
        yield return new WaitForSeconds(.1f);
        pointLight.enabled = false;
        spotLight.enabled = false;
        yield return new WaitForSeconds(1f);
        pointLight.enabled = true;
        spotLight.enabled = true;
        pointLight.intensity = 0;
        spotLight.intensity = 0;

        float s = 0;
        while (s < 30)
        {
            s += 3;
            pointLight.intensity = 6 / (31 - s);
            spotLight.intensity = 20 / (31 - s);
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }
}
