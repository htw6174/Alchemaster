using UnityEngine;
using System.Collections;

public class Barrel : Container {

    public Connector outputPipe;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(outputPipe.transform.position, 0.2f);
    }
}
