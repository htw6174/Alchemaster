using UnityEngine;
using System.Collections;

public class PipeCreator : MonoBehaviour {

    public float curveRadius, pipeRadius;

    public int curveSegmentCount, pipeSegmentCount;

    public float directionLength;

    public Vector3[] points;

    public void Reset()
    {
        points = new Vector3[] {
            new Vector3(0f, 0f, 0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f)
        };
    }

    private Vector3 GetPointOnPipe(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
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
        Vector3 p;
        float r = pipeRadius * Mathf.Cos(v);
        p.x = 0f;
        p.y = r;
        p.z = pipeRadius * Mathf.Sin(v);

        //float r = curveRadius + pipeRadius * Mathf.Cos(v);
        //p.x = r * Mathf.Sin(u);
        //p.y = r * Mathf.Cos(u);
        //p.z = pipeRadius * Mathf.Sin(v);
        return p;
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
        return GetFirstDerivative(points[0], points[1], points[2], points[3], t) - transform.position;
    }

    private Vector3 GetDirection(float t, float length = 1f)
    {
        return GetVelocity(t).normalized * length;
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
            Vector3 ringCenter = GetPointOnPipe(points[0], points[1], points[2], points[3], pipeProgress) + transform.position;
            Gizmos.DrawSphere(ringCenter, 0.1f);
            Gizmos.DrawRay(ringCenter, GetDirection(pipeProgress, directionLength));
            for (int v = 0; v < pipeSegmentCount; v++)
            {
                Vector3 point = GetPointOnRing(GetDirection(pipeProgress), v * vStep);
                Gizmos.color = new Color(0f, (float)v / pipeSegmentCount, pipeProgress);
                Gizmos.DrawSphere(point + ringCenter, 0.1f);
            }
        }
    }
}
