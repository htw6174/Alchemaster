using UnityEngine;
using System.Collections;

public class Pipe : MonoBehaviour {

    public Connector source;
    public Connector destination;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(source.transform.position, destination.transform.position);
    }
}
