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

    // Центр между двумя точками
    Vector3 mid = (posA + posB) / 2f;

    // Направление от A к B
    Vector3 direction = (posB - posA).normalized;

    // Вектор, перпендикулярный направлению (в плоскости XY, можно заменить на Cross с Vector3.up если нужно в другой плоскости)
    Vector3 perpendicular = -Vector3.Cross(direction, Vector3.forward).normalized;

    // Модификатор длины дуги
    float curveStrength = 0.3f;

    // Контрольная точка — смещённая относительно середины
    Vector3 controlPoint = mid + perpendicular * curveStrength;

    // Количество сегментов кривой
    int segments = 20;
    lr.positionCount = segments + 1;

    // Вычисление точек кривой Безье
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
