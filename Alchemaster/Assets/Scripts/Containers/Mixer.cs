using UnityEngine;
using System.Collections;

public class Mixer : Container {

    public Connector inputOne;
    public Connector inputTwo;
    public Connector outputPipe;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(inputOne.transform.position, 0.2f);
        Gizmos.DrawSphere(inputTwo.transform.position, 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(outputPipe.transform.position, 0.2f);
    }
}
