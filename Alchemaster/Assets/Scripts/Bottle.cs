using UnityEngine;
using System.Collections;

public class Bottle : Container {

    public Connector inputPipe;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(inputPipe.transform.position, 0.2f);
    }
}
