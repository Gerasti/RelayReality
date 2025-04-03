using UnityEngine;
using UnityEngine.UI;

public class CableConnector : MonoBehaviour
{
    public Camera mainCamera;
    public Transform temporaryPoint; // Empty перед камерой
    public Button connectButton; // UI-кнопка
    public LayerMask connectionLayer; // Слой точек соединения
    public int curveResolution = 20;
    public float curveOffset = 1.0f;

    private LineRenderer lineRenderer;
    private Transform firstPoint;
    private Transform secondPoint;
    private bool isConnecting = false;
    private bool isTemporaryActive = false;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = curveResolution;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.enabled = false;

        connectButton.onClick.AddListener(ToggleConnecting);
    }

    void Update()
    {
        if (!isConnecting) return;

        if (firstPoint == null)
        {
            CheckForConnectionPoint();
        }
        else if (!isTemporaryActive)
        {
            isTemporaryActive = true;
        }
        else
        {
            CheckForSecondPoint();
            UpdateCable(firstPoint.position, isTemporaryActive ? temporaryPoint.position : secondPoint.position);
        }
    }

    void ToggleConnecting()
    {
        isConnecting = !isConnecting;
        firstPoint = null;
        secondPoint = null;
        isTemporaryActive = false;
        lineRenderer.enabled = isConnecting;
    }

    void CheckForConnectionPoint()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, connectionLayer))
            {
                firstPoint = hit.transform;
                isTemporaryActive = true;
            }
        }
    }

    void CheckForSecondPoint()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, connectionLayer) && hit.transform != firstPoint)
            {
                secondPoint = hit.transform;
                isTemporaryActive = false;
            }
        }
    }

    void UpdateCable(Vector3 start, Vector3 end)
    {
        Vector3 mid = (start + end) / 2 - Vector3.up * curveOffset;
        for (int i = 0; i < curveResolution; i++)
        {
            float t = i / (float)(curveResolution - 1);
            Vector3 pos = Bezier(start, mid, end, t);
            lineRenderer.SetPosition(i, pos);
        }
    }

    Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
    }
}
