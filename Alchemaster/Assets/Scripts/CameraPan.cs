using UnityEngine;
using System.Collections;

public class CameraPan : MonoBehaviour
{
    public GameObject table;

    public Vector3 maxCamPos;
    public Vector3 minCamPos;

    public Vector3 currentMousePos = Vector3.zero;
    public Vector3 lastMousePos = Vector3.zero;

    public bool dragging = false;

    void Update()
    {
        currentMousePos = Input.mousePosition;
        currentMousePos.z = transform.position.z;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 unvertedMousePos = Camera.main.ScreenToWorldPoint(new Vector3(currentMousePos.x, currentMousePos.y, -Camera.main.nearClipPlane));
            Ray clickCast = new Ray(Camera.main.transform.position, Camera.main.transform.position - unvertedMousePos);
            Debug.DrawRay(clickCast.origin, clickCast.direction);
            RaycastHit hit;
            if (Physics.Raycast(clickCast, out hit))
            {
                Debug.DrawLine(clickCast.origin, hit.point);
                Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject == table)
                {
                    dragging = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }

        if (dragging)
        {
            Vector3 currentMouseWorld = Camera.main.ScreenToWorldPoint(currentMousePos);
            Vector3 lastMouseWorld = Camera.main.ScreenToWorldPoint(lastMousePos);
            Vector3 cameraDelta = new Vector3(lastMouseWorld.x - currentMouseWorld.x, lastMouseWorld.y - currentMouseWorld.y, 0f);
            Vector3 newCamPosition = transform.position - cameraDelta;
            newCamPosition.x = Mathf.Clamp(newCamPosition.x, minCamPos.x, maxCamPos.x);
            newCamPosition.y = Mathf.Clamp(newCamPosition.y, minCamPos.y, maxCamPos.y);
            newCamPosition.z = Mathf.Clamp(newCamPosition.z, minCamPos.z, maxCamPos.z);
            transform.position = newCamPosition;
        }

        lastMousePos = currentMousePos;
        lastMousePos.z = transform.position.z;
    }
}
