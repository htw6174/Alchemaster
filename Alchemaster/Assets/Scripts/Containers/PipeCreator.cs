using UnityEngine;
using System.Collections;

[RequireComponent(typeof (MeshFilter), typeof (MeshRenderer))]
public class PipeCreator : MonoBehaviour {

    public float pipeRadius;

    public int segmentCount, sideCount;

    public float directionLength;
    public float pointOffset;
    public float minCurvature = 10f;
    public float medCurvature = 30f;
    public float maxCurvature = 40f;

    public Vector3 startDirection;
    public Vector3 endDirection;

    public bool autoControlPoints = true;
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

    public void InitializePipe(Vector3 start, Vector3 end, Vector3 startDirection, Vector3 endDirection, float radius, int segments = 20, int sides = 10)
    {
        points = new Vector3[] { start, Vector3.zero, Vector3.zero, end };
        this.startDirection = startDirection;
        this.endDirection = endDirection;
        pipeRadius = radius;
        segmentCount = segments;
        sideCount = sides;
        UpdatePipeMesh();
    }

    public void UpdatePipe(Vector3 newStart, Vector3 newEnd)
    {
        points[0] = newStart;
        points[3] = newEnd;
        UpdatePipeMesh();
    }

    private void Update()
    {
        UpdatePipeMesh();
    }

    private void UpdatePipeMesh()
    {
        if(autoControlPoints) PlaceControlPoints();
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Pipe";
        SetVertices();
        SetTriangles();
        mesh.RecalculateNormals();
    }

    private void PlaceControlPoints()
    {
        float pipeCurvature = DetermineCurvature(startDirection, endDirection * -1f);

        float deltaX = Mathf.Abs(points[3].x - points[0].x);
        float deltaY = Mathf.Abs(points[3].y - points[0].y);

        //float p1x = startDirection.x * pointOffset * deltaX;
        //float p1y = startDirection.y * pointOffset * deltaY;
        //float p2x = endDirection.x * pointOffset * deltaX;
        //float p2y = endDirection.y * pointOffset * deltaY;

        float p1x = MaxAbsolute(pipeCurvature * pipeRadius, pointOffset * deltaX) * startDirection.x;
        float p1y = MaxAbsolute(pipeCurvature * pipeRadius, pointOffset * deltaY) * startDirection.y;
        float p2x = MaxAbsolute(pipeCurvature * pipeRadius, pointOffset * deltaX) * endDirection.x;
        float p2y = MaxAbsolute(pipeCurvature * pipeRadius, pointOffset * deltaY) * endDirection.y;

        points[1] = new Vector3(p1x, p1y, 0f) + points[0];
        points[2] = new Vector3(p2x * -1f, p2y * -1f, 0f) + points[3];

        //float midpointX = (points[0].x + points[3].x) / 2f;
        //points[1].x = midpointX;
        //points[2].x = midpointX;

        //float deltaY = points[3].y - points[0].y;
        //float controlY = deltaY > 0f ? -1f : 1f;
        //points[1].y = controlY + points[0].y;
        //points[2].y = -controlY + points[3].y;

        //points[1].z = points[0].z;
        //points[2].z = points[3].z;
    }

    private float DetermineCurvature(Vector3 p0, Vector3 p1)
    {
        float endpointsAngle = Vector3.Angle(p0, p1);
        if (endpointsAngle < 30f)
        {
            return Mathf.Lerp(minCurvature, medCurvature, )
            if (Vector3.Distance(points[0], points[3]) < medCurvature * pipeRadius) return minCurvature;
            else return medCurvature;
        }
        else if (endpointsAngle < 90f) return minCurvature;
        else if (endpointsAngle >= 180f) return minCurvature;
        else if (endpointsAngle >= 90f)
        {
            Vector3 difference = p0 - p1;
            if ((points[3].x - points[0].x * difference.x > 0f) && (points[3].y - points[0].y * difference.y > 0f)) return minCurvature;
            else return maxCurvature;
        }

        return minCurvature;
    }

    private float MaxAbsolute(float a, float b)
    {
        if(Mathf.Abs(a) > Mathf.Abs(b)) return a;
        else return b;
    }

    private void SetVertices()
    {
        vertices = new Vector3[sideCount * segmentCount * 4];
        float uStep = 1f / segmentCount;
        CreateFirstQuadRing(uStep);
        int iDelta = sideCount * 4;
        for (int u = 2, i = iDelta; u <= segmentCount; u++, i += iDelta)
        {
            CreateQuadRing(u * uStep, i);
        }
        mesh.vertices = vertices;
    }

    private void CreateFirstQuadRing(float u)
    {
        float vStep = (2f * Mathf.PI) / sideCount;

        Vector3 vertexA = GetPointOnPipe(0f, 0f);
        Vector3 vertexB = GetPointOnPipe(u, 0f);
        for (int v = 1, i = 0; v <= sideCount; v++, i += 4)
        {
            vertices[i] = vertexA;
            vertices[i + 1] = vertexA = GetPointOnPipe(0f, v * vStep);
            vertices[i + 2] = vertexB;
            vertices[i + 3] = vertexB = GetPointOnPipe(u, v * vStep);
        }
    }

    private void CreateQuadRing(float u, int i)
    {
        float vStep = (2f * Mathf.PI) / sideCount;
        int ringOffset = sideCount * 4;

        Vector3 vertex = GetPointOnPipe(u, 0f);
        for (int v = 1; v <= sideCount; v++, i += 4)
        {
            vertices[i] = vertices[i - ringOffset + 2];
            vertices[i + 1] = vertices[i - ringOffset + 3];
            vertices[i + 2] = vertex;
            vertices[i + 3] = vertex = GetPointOnPipe(u, v * vStep);
        }
    }

    private void SetTriangles()
    {
        triangles = new int[sideCount * segmentCount * 6];
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

        float r = pipeRadius * Mathf.Cos(v) *direction.x;
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

    private Vector3 GetVelocity(float u)
    {
        return GetFirstDerivative(points[0], points[1], points[2], points[3], u);
    }

    private Vector3 GetDirection(float u, float length = 1f)
    {
        if (u == 0f)
        {
            return startDirection.normalized * length;
        }
        else if (u == 1f)
        {
            return endDirection.normalized * length;
        }
        else
        {
            return GetVelocity(u).normalized * length;
        }
    }

    private void OnDrawGizmos()
    {
        float vStep = (2f * Mathf.PI) / sideCount;

        foreach (Vector3 point in points)
        {
            Gizmos.DrawSphere(point + transform.position, 0.4f);
        }

        for (int u = 0; u <= segmentCount; u++)
        {
            float pipeProgress = (float)u / segmentCount;
            Gizmos.color = new Color(pipeProgress, 0f, 0f);
            Vector3 ringCenter = GetMidpointInPipe(points[0], points[1], points[2], points[3], pipeProgress) + transform.position;
            Gizmos.DrawSphere(ringCenter, 0.1f);
            Gizmos.DrawRay(ringCenter, GetDirection(pipeProgress, directionLength));
            for (int v = 0; v < sideCount; v++)
            {
                Vector3 point = GetPointOnPipe(pipeProgress, v * vStep);
                Gizmos.color = new Color(0f, (float)v / sideCount, pipeProgress);
                //Gizmos.DrawSphere(point + transform.position, 0.1f);
            }
        }
        UpdatePipeMesh();
    }
}
