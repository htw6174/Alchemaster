using UnityEngine;
using System.Collections;

public class Connector : MonoBehaviour
{
    public Container parentContainer;
    public Fluid allowedFluid;

    public bool input;
    public bool output;

    public Vector3 direction;

    public Connector otherEnd;

    public GameObject pipePrefab;
    public PipeCreator pipe;
    public float pipeRadius;

    private Vector3 lastPos;

    void Start()
    {
        lastPos = transform.position;
        parentContainer = transform.parent.gameObject.GetComponent<Container>();
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, lastPos) > 0.0001f)
        {
            if (pipe)
            {
                UpdatePipe();
            }
            else if (otherEnd && otherEnd.pipe)
            {
                otherEnd.UpdatePipe();
            }
        }
        lastPos = transform.position;
    }

    public bool MakeConnection(Connector other)
    {
        if (other.input == false && input == false) return false;
        if (other.output == false && output == false) return false;
        if (other.input == false && output == false) return false;
        if (other.otherEnd)
        {
            other.otherEnd.otherEnd = null;
            other.otherEnd = this;
            other.otherEnd.otherEnd = other;
        }
        else
        {
            other.otherEnd = this;
            other.otherEnd.otherEnd = other;
        }
        Debug.Log("Made new connection with " + other.gameObject.name);
        ReplacePipe();
        return true;
    }

    public void UpdatePipe()
    {
        pipe.UpdatePipe(transform.position, otherEnd.transform.position);
    }

    public void ReplacePipe()
    {
        DestroyPipe();
        if (otherEnd)
        {
            if (otherEnd.pipe) otherEnd.DestroyPipe();
            CreatePipe();
        }
    }

    public void DestroyPipe()
    {
        if (pipe) Destroy(pipe.gameObject);
    }

    private void CreatePipe()
    {
        pipe = (Instantiate<GameObject>(pipePrefab) as GameObject).GetComponent<PipeCreator>();
        pipe.InitializePipe(transform.position, otherEnd.transform.position, direction, -otherEnd.direction, pipeRadius);
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
        else
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(transform.position, 0.2f);
        }
        if (otherEnd)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, otherEnd.transform.position);
        }
    }
}
