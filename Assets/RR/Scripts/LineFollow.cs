using UnityEngine;

public class LineFollow : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    private LineRenderer lr;
    private Vector3 lastA;
    private Vector3 lastB;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        UpdateLine();
    }

    void LateUpdate()
{
    Vector3 currentA = pointA.position;
    Vector3 currentB = pointB.position;

    if (currentA != lastA || currentB != lastB)
    {
        ConnectionManager connectionManager = FindObjectOfType<ConnectionManager>();
        if (connectionManager != null)
        {
            connectionManager.ResetSelection(); 
            connectionManager.RemoveConnection(pointA, pointB);

        }

        lr.positionCount = 0;
        lastA = currentA;
        lastB = currentB;
    }
}


public void Initialize(Transform newA, Transform newB)
{
    pointA = newA;
    pointB = newB;
    lr = GetComponent<LineRenderer>();
    lastA = pointA.position;
    lastB = pointB.position;
    UpdateLine();
}

 void UpdateLine()
{
    Vector3 posA = pointA.position;
    Vector3 posB = pointB.position;

    Vector3 mid = (posA + posB) / 2f;


    Vector3 direction = (posB - posA).normalized;

    Vector3 perpendicular = -Vector3.Cross(direction, Vector3.forward).normalized;


    float curveStrength = 0.3f;


    Vector3 controlPoint = mid + perpendicular * curveStrength;

    int segments = 20;
    lr.positionCount = segments + 1;

    for (int i = 0; i <= segments; i++)
    {
        float t = i / (float)segments;
        Vector3 bezierPoint = CalculateQuadraticBezierPoint(t, posA, controlPoint, posB);
        lr.SetPosition(i, bezierPoint);
    }

    lastA = posA;
    lastB = posB;
}
Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
{
    return Mathf.Pow(1 - t, 2) * p0 +
           2 * (1 - t) * t * p1 +
           Mathf.Pow(t, 2) * p2;
}

}
