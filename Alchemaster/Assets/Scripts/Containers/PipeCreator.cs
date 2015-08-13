using UnityEngine;
using System.Collections;

public class PipeCreator : MonoBehaviour {

    public float curveRadius, pipeRadius;

    public int curveSegmentCount, pipeSegmentCount;

    public float directionLength;

    public Vector3 startDirection;
    public Vector3 endDirection;
    public Vector3[] points;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    public void Reset()
    {
        points = new Vector3[] {
            new Vector3(0f, 0f, 0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f)
        };
    }

    private void Update()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Pipe";
        SetVertices();
        SetTriangles();
    }

    private void SetVertices()
    {
        vertices = new Vector3[pipeSegmentCount * curveSegmentCount * 4];
        float uStep = 1f / curveSegmentCount;
        CreateFirstQuadRing(uStep);
        int iDelta = pipeSegmentCount * 4;
        for (int u = 2, i = iDelta; u <= curveSegmentCount; u++, i += iDelta)
        {
            CreateQuadRing(u * uStep, i);
        }
        mesh.vertices = vertices;
    }

    private void CreateFirstQuadRing(float u)
    {
        float vStep = (2f * Mathf.PI) / pipeSegmentCount;

        Vector3 vertexA = GetPointOnPipe(0f, 0f);
        Vector3 vertexB = GetPointOnPipe(u, 0f);
        for (int v = 1, i = 0; v <= pipeSegmentCount; v++, i += 4)
        {
            vertices[i] = vertexA;
            vertices[i + 1] = vertexA = GetPointOnPipe(0f, v * vStep);
            vertices[i + 2] = vertexB;
            vertices[i + 3] = vertexB = GetPointOnPipe(u, v * vStep);
        }
    }

    private void CreateQuadRing(float u, int i)
    {
        float vStep = (2f * Mathf.PI) / pipeSegmentCount;
        int ringOffset = pipeSegmentCount * 4;

        Vector3 vertex = GetPointOnPipe(u, 0f);
        for (int v = 1; v <= pipeSegmentCount; v++, i += 4)
        {
            vertices[i] = vertices[i - ringOffset + 2];
            vertices[i + 1] = vertices[i - ringOffset + 3];
            vertices[i + 2] = vertex;
            vertices[i + 3] = vertex = GetPointOnPipe(u, v * vStep);
        }
    }

    private void SetTriangles()
    {
        triangles = new int[pipeSegmentCount * curveSegmentCount * 6];
        for (int t = 0, i = 0; t < triangles.Length; t += 6, i += 4)
        {
            triangles[t] = i;
            triangles[t + 1] = triangles[t + 4] = i + 1;
            triangles[t + 2] = triangles[t + 3] = i + 2;
            triangles[t + 5] = i + 3;
        }
        mesh.triangles = triangles;
    }

    private Vector3 GetMidpointInPipe(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            (oneMinusT * oneMinusT * oneMinusT * p0) +
            (3f * oneMinusT * oneMinusT * t * p1) +
            (3f * oneMinusT * t * t * p2) +
            (t * t * t * p3);
    }

    private Vector3 GetPointOnRing(Vector3 direction, float v)
    {
        float angle = Vector3.Angle(Vector3.right, direction);
        angle = angle * Mathf.Deg2Rad;
        Vector3 p;

        float r = pipeRadius * Mathf.Cos(v);
        p.x = -Mathf.Cos(v) * direction.y * pipeRadius;
        p.y = r;
        p.z = pipeRadius * Mathf.Sin(v);

        p = Vector3.ProjectOnPlane(p, direction);
        return p;
    }

    private Vector3 GetPointOnPipe(float u, float v)
    {
        Vector3 midpoint = GetMidpointInPipe(points[0], points[1], points[2], points[3], u);
        Vector3 ringpoint = GetPointOnRing(GetDirection(u), v);
        return ringpoint + midpoint;
    }

    private Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            3f * oneMinusT * oneMinusT * (p1 - p0) +
            6f * oneMinusT * t * (p2 - p1) +
            3f * t * t * (p3 - p2);
    }

    private Vector3 GetVelocity(float t)
    {
        return GetFirstDerivative(points[0], points[1], points[2], points[3], t);
    }

    private Vector3 GetDirection(float t, float length = 1f)
    {
        if (t == 0f)
        {
            return startDirection.normalized * length;
        }
        else if (t == 1f)
        {
            return endDirection.normalized * length;
        }
        else
        {
            return GetVelocity(t).normalized * length;
        }
    }

    private void OnDrawGizmos()
    {
        float vStep = (2f * Mathf.PI) / pipeSegmentCount;

        foreach (Vector3 point in points)
        {
            Gizmos.DrawSphere(point + transform.position, 0.4f);
        }

        for (int u = 0; u <= curveSegmentCount; u++)
        {
            float pipeProgress = (float)u / curveSegmentCount;
            Gizmos.color = new Color(pipeProgress, 0f, 0f);
            Vector3 ringCenter = GetMidpointInPipe(points[0], points[1], points[2], points[3], pipeProgress) + transform.position;
            Gizmos.DrawSphere(ringCenter, 0.1f);
            Gizmos.DrawRay(ringCenter, GetDirection(pipeProgress, directionLength));
            for (int v = 0; v < pipeSegmentCount; v++)
            {
                Vector3 point = GetPointOnPipe(pipeProgress, v * vStep);
                Gizmos.color = new Color(0f, (float)v / pipeSegmentCount, pipeProgress);
                Gizmos.DrawSphere(point + transform.position, 0.1f);
            }
        }
    }
}
