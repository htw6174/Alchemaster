using UnityEngine;
using System.Collections;

public class Connector : MonoBehaviour
{
    public Container parentContainer;
    public Fluid allowedFluid;

    public bool input;
    public bool output;

    public Connector otherEnd;

    void Start()
    {
        parentContainer = transform.parent.gameObject.GetComponent<Container>();
    }

    //Attempts to send fluid to otherEnd.parentContainer, returns the volume of fluid successfully sent
    public int OutputFluid(int volume, Fluid fluid)
    {
        if (otherEnd)
        {
            if (otherEnd.input == true)
            {
                return otherEnd.parentContainer.InputFluid(volume, fluid);
            }
        }
        return 0;
    }

    void OnDrawGizmos()
    {
        if (input && output)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position, 0.2f);
        }
        else if (input)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 0.2f);
        }
        else if (output)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.2f);
        }
        if (otherEnd)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, otherEnd.transform.position);
        }
    }
}
