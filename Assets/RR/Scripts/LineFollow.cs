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
        if (pointA) lastA = pointA.position;
        if (pointB) lastB = pointB.position;
        lr.SetPosition(0, lastA);
        lr.SetPosition(1, lastB);
    }

    void LateUpdate()
    {
        Vector3 currentA = pointA.position;
        Vector3 currentB = pointB.position;

        if (currentA != lastA || currentB != lastB)
        {
            lr.SetPosition(0, currentA);
            lr.SetPosition(1, currentB);
            lastA = currentA;
            lastB = currentB;
        }
    }
}
