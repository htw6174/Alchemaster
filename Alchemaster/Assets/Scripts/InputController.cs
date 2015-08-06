using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

    public Vector3 currentMousePos;

    public bool placingConnector = false;

    void Update()
    {
        currentMousePos = Input.mousePosition;
        currentMousePos.z = transform.position.z;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit clickHit = ClickCast();

            if (clickHit.transform.gameObject)
            {
                GameObject target = clickHit.transform.gameObject;

                if (target.tag == Tags.draggable)
                {
                    Debug.Log("Clicked a draggable object!");
                }

                if (target.tag == Tags.connector)
                {
                    Connector selectedConnector = target.GetComponent<Connector>();

                    if (selectedConnector.otherEnd)
                    {
                        selectedConnector.otherEnd.otherEnd = null;
                        selectedConnector.otherEnd = null;
                    }
                    placingConnector = true;
                    Debug.Log("Making new connection...");


                }
            }
        }
    }

    private RaycastHit ClickCast()
    {
        RaycastHit clickHit;
        Vector3 unvertedMousePos = Camera.main.ScreenToWorldPoint(new Vector3(currentMousePos.x, currentMousePos.y, -Camera.main.nearClipPlane));
        Ray clickCast = new Ray(Camera.main.transform.position, Camera.main.transform.position - unvertedMousePos);
        Debug.DrawRay(clickCast.origin, clickCast.direction);
        bool hit = Physics.Raycast(clickCast, out clickHit);

        if (hit)
        {
            Debug.DrawLine(clickCast.origin, clickHit.point);
            Debug.Log(clickHit.transform.gameObject.name);
        }

        return clickHit;
    }

    private void PlaceConnector()
    {

    }
}
