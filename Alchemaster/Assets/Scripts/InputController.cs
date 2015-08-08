using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

    public GameObject camera;

    public Vector3 currentMousePos = Vector3.zero;
    public Vector3 lastMousePos = Vector3.zero;

    public float scrollSensitivity = 10f;

    public GameObject target; //Object that was hit when the mouse was pressed
    public GameObject heldObject; //Last pickable object to be selected
    public Connector firstConnector; //First connector selected when making a new connection

    public bool holdingObject = false; //Consider replacing with check to see if relevant object exists
    public bool placingConnector = false;

    public Vector3 maxCamPos;
    public Vector3 minCamPos;

    public bool dragging = false;

    void Update()
    {
        currentMousePos = Input.mousePosition;

        CheckSelectInput();

        CheckPanInput();

        CheckScrollInput();

        if (holdingObject)
        {
            Vector3 heldPosition = WorldMousePos(currentMousePos, Mathf.Abs(Camera.main.transform.position.z - heldObject.transform.position.z));
            heldPosition.z = heldObject.transform.position.z;
            heldObject.transform.position = heldPosition;
        }

        if (dragging)
        {
            Vector3 currentMouseWorld = WorldMousePos(currentMousePos, camera.transform.position.z);
            Vector3 lastMouseWorld = WorldMousePos(lastMousePos, camera.transform.position.z);
            MoveCamera(new Vector3(lastMouseWorld.x - currentMouseWorld.x, lastMouseWorld.y - currentMouseWorld.y, 0f));
        }

        lastMousePos = currentMousePos;
    }

    void OnDrawGizmos()
    {
        if (placingConnector)
        {
            Gizmos.DrawLine(firstConnector.transform.position, WorldMousePos(currentMousePos, Mathf.Abs(Camera.main.transform.position.z - firstConnector.transform.position.z)));
        }
    }

    private void CheckSelectInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit clickHit;

            if (Physics.Raycast(ClickRay(currentMousePos), out clickHit))
            {
                target = clickHit.transform.gameObject;

                if (holdingObject)
                {
                    Debug.Log("Trying to drop a draggable object...");
                    if (CheckPlacement(heldObject))
                    {
                        heldObject = null;
                        holdingObject = false;
                        Debug.Log("Dropped a draggable object!");
                    }
                }
                else
                {
                    if (target.tag == Tags.draggable)
                    {
                        Debug.Log("Clicked a draggable object!");
                        heldObject = target.transform.gameObject;
                        holdingObject = true;
                    }
                }

                if (target.tag == Tags.connector)
                {
                    Connector hitConnecter = target.GetComponent<Connector>();

                    if (placingConnector)
                    {
                        PlaceConnector(hitConnecter);
                    }
                    else
                    {
                        SelectConnector(hitConnecter);
                    }
                }
            }
        }
    }

    private void CheckPanInput()
    {
        dragging = Input.GetMouseButton(1);
    }

    private void CheckScrollInput()
    {
        MoveCamera(new Vector3(0f, 0f, Input.GetAxis("Mouse ScrollWheel")) * scrollSensitivity * -1f);
    }

    public Vector3 Vec3Clamp(Vector3 value, Vector3 min, Vector3 max)
    {
        Vector3 clampedVector = Vector3.zero;
        clampedVector.x = Mathf.Clamp(value.x, min.x, max.x);
        clampedVector.y = Mathf.Clamp(value.y, min.y, max.y);
        clampedVector.z = Mathf.Clamp(value.z, min.z, max.z);
        return clampedVector;
    }

    public void MoveCamera(Vector3 cameraDelta)
    {
        Vector3 newCamPosition = camera.transform.position - cameraDelta;
        camera.transform.position = Vec3Clamp(newCamPosition, minCamPos, maxCamPos);
    }

    //Ray from camera position to mouse world position on the near clipping plane of the camera
    public Ray ClickRay(Vector3 mousePos)
    {
        RaycastHit clickHit;
        Vector3 worldMousePos = WorldMousePos(mousePos, -Camera.main.nearClipPlane);
        Ray clickCast = new Ray(Camera.main.transform.position, Camera.main.transform.position - worldMousePos);

        //Depreciated
        bool hit = Physics.Raycast(clickCast, out clickHit);
        if (hit)
        {
            Debug.DrawLine(clickCast.origin, clickHit.point);
            Debug.Log(clickHit.transform.gameObject.name);
        }

        return clickCast;
    }

    //Mouse world position on the near clipping plane of the camera
    public Vector3 WorldMousePos(Vector3 mousePos, float distance = 0f)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, distance));
    }

    //Check if heldObject overlaps with any other objects when placed
    private bool CheckPlacement(GameObject held)
    {
        return true;
    }

    //Removes old connections on the selected connector, sets it as first of two ends
    private void SelectConnector(Connector selected)
    {
        if (selected.input == false && selected.output == false) return;
        if (selected.otherEnd)
        {
            selected.otherEnd.otherEnd = null;
            selected.otherEnd = null;
        }
        firstConnector = selected;
        placingConnector = true;
        Debug.Log("Making new connection with " + selected.gameObject.name);
    }

    //Removes old connections on selected connector, sets it as the second of two ends
    private void PlaceConnector(Connector selected)
    {
        if (selected == firstConnector)
        {
            firstConnector = null;
            placingConnector = false;
            Debug.Log("Canceled placing of connector");
            return;
        }
        if (selected.input == false && firstConnector.input == false) return;
        if (selected.output == false && firstConnector.output == false) return;
        if (selected.input == false && selected.output == false) return;
        if (selected.otherEnd)
        {
            selected.otherEnd.otherEnd = null;
            selected.otherEnd = firstConnector;
            selected.otherEnd.otherEnd = selected;
        }
        else
        {
            selected.otherEnd = firstConnector;
            selected.otherEnd.otherEnd = selected;
        }
        placingConnector = false;
        Debug.Log("Made new connection with " + selected.gameObject.name);
    }
}
