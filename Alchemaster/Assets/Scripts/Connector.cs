using UnityEngine;
using System.Collections;
using Fluids;

public class Connector : MonoBehaviour
{
    public Fluid allowedFluid;

    public bool input;
    public bool output;

    public Connector otherEnd;

    void OnDrawGizmos()
    {
        if (otherEnd)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, otherEnd.transform.position);
        }
    }
}
