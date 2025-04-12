using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public Material lineMaterial;
    public float lineWidth = 0.02f;

    private GameObject firstPoint;
    private bool connectModeActive = false;

    public void SetConnectMode(bool active)
    {
        connectModeActive = active;
        if (!active) firstPoint = null;
    }

    [SerializeField]private bool IsConnectModeActive()
    {
        return connectModeActive;
    }

    void Update()
    {
        if (!connectModeActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("ConnectionPoint"))
                {
                    if (firstPoint == null)
                    {
                        firstPoint = hit.collider.gameObject;
                    }
                    else
                    {
                        GameObject secondPoint = hit.collider.gameObject;
                        if (secondPoint != firstPoint)
                        {
                            CreateLine(firstPoint.transform, secondPoint.transform);
                            firstPoint = null;
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            firstPoint = null;
        }
    }

    void CreateLine(Transform pointA, Transform pointB)
    {
        GameObject lineObj = new GameObject("Line");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = 2;

        lr.SetPosition(0, pointA.position);
        lr.SetPosition(1, pointB.position);

        LineFollow follow = lineObj.AddComponent<LineFollow>();
        follow.pointA = pointA;
        follow.pointB = pointB;
    }
}
