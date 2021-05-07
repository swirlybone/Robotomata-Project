using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NotificationCenter;

public class CameraController : MonoBehaviour
{
    public enum State { ZOOMED, UNZOOMED }
    State state = State.UNZOOMED;

    [SerializeField] float speed = .1f;
    [SerializeField] GameObject cameraRig;
    [SerializeField] [Range(0, 2)] float sensitivity;

    Vector3 origin;

    GameObject LookAtPoint;
    new Camera camera;
    Quaternion originalLook;

    float FOV;

    private void OnEnable()
    {
        AddObserver("Zoom", Zoom);
        AddObserver("UnZoom", UnZoom);
    }

    private void OnDisable()
    {
        RemoveObserver("Zoom", Zoom);
        RemoveObserver("UnZoom", UnZoom);
    }

    private void Start()
    {
        camera = cameraRig.GetComponent<Camera>();
        originalLook = camera.transform.rotation;
        FOV = camera.fieldOfView;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt) && Input.mousePosition != origin)
        {
            AdjustCamera();
        }

        if (Zoomed())
        {
            cameraRig.transform.LookAt(LookAtPoint.transform.position + new Vector3(0,1,0));
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt)) origin = Input.mousePosition;

    }

    private void FixedUpdate()
    {
        if (Input.GetKey("w")) transform.Translate(Vector3.forward * speed, Space.Self);
        if (Input.GetKey("s")) transform.Translate(Vector3.back * speed, Space.Self);
        if (Input.GetKey("a")) transform.Translate(Vector3.left * speed, Space.Self);
        if (Input.GetKey("d")) transform.Translate(Vector3.right * speed, Space.Self);
        if (Input.GetKey("q")) transform.Translate(Vector3.down * speed, Space.Self);
        if (Input.GetKey("e")) transform.Translate(Vector3.up * speed, Space.Self);
        if (Input.GetKey(KeyCode.UpArrow)) transform.Rotate(Vector3.right, (5 * -speed), Space.Self);
        if (Input.GetKey(KeyCode.DownArrow)) transform.Rotate(Vector3.right, (5 * speed), Space.Self);
        if (Input.GetKey(KeyCode.LeftArrow)) transform.Rotate(Vector3.up, (5 * -speed), Space.World);
        if (Input.GetKey(KeyCode.RightArrow)) transform.Rotate(Vector3.up, (5 * speed), Space.World);
    }

    void Zoom(Notification notification)
    {
        GameObject thing = (GameObject)notification.Object;
        LookAtPoint = thing;
        StartCoroutine(Zooming(20));
        state = State.ZOOMED;
    }

    void UnZoom(Notification notification)
    {
        StopAllCoroutines();
        state = State.UNZOOMED;
        camera.transform.rotation = originalLook;
        camera.fieldOfView = FOV;
    }

    IEnumerator Zooming(float zoomAmount)
    {
        while (camera.fieldOfView > zoomAmount)
        {
            camera.fieldOfView -= speed;

            yield return null;
        }
    }

    bool Zoomed() => state == State.ZOOMED;

    void AdjustCamera()
    {
        if (Input.mousePosition.y > origin.y) CameraMove(Vector3.left, Space.Self, 1.08f);
        if (Input.mousePosition.y < origin.y) CameraMove(Vector3.right, Space.Self, 1.08f);
        if (Input.mousePosition.x > origin.x) CameraMove(Vector3.up, Space.World, 1.92f);
        if (Input.mousePosition.x < origin.x) CameraMove(Vector3.down, Space.World, 1.92f);

        origin = Vector3.MoveTowards(origin, Input.mousePosition, 100);
    }

    void CameraMove(Vector3 direction, Space space, float multiplier)
    {
        camera.transform.Rotate(direction * sensitivity * multiplier, space);
    }
}
