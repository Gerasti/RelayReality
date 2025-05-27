using UnityEngine;
using System.Collections.Generic;

public class ConnectionManager : MonoBehaviour
{
    public Material lineMaterial;
    public static float lineWidth = 0.08f;

    public GameObject firstPoint;
    private bool connectModeActive = false;

    public Material backlightMaterial;
    private Material originalMaterial;
    private Renderer firstPointRenderer;

    public Dictionary<Transform, List<Transform>> connectionMap = new();
    private List<GameObject> lines_buffer = new();
    public Dictionary<(Transform, Transform), GameObject> activeLines = new();

    private float delay = 2f;
    private float timer = 0f;
    private bool started = false;

    void Start()
    {
        started = true;
        timer = 0f;
    }

    public void SetPulseActiveForAll(bool active) { }

    void TryAutoConnect(string nameA, string nameB)
    {
        GameObject objA = GameObject.Find(nameA);
        GameObject objB = GameObject.Find(nameB);

        if (objA == null || objB == null)
        {
            Debug.LogWarning($"One or both objects not found: {nameA}, {nameB}");
            return;
        }

        Transform a = objA.transform;
        Transform b = objB.transform;

        if (!ConnectionExists(a, b))
        {
            CreateLine(a, b);
            RegisterConnection(a, b);
            Debug.Log($"Auto-connected: {nameA} <-> {nameB}");
        }
        else
        {
            Debug.Log($"Connection already exists: {nameA} <-> {nameB}");
        }
    }





    public void ResetSelection()
    {
        if (firstPointRenderer != null)
            firstPointRenderer.material = originalMaterial;

        firstPoint = null;
        firstPointRenderer = null;
    }

    public void SetConnectMode(bool active)
    {
        connectModeActive = active;
        Debug.Log("Connect Mode: " + (active ? "ON" : "OFF"));

        if (!active)
        {
            ResetSelection();
            Debug.Log("First point reset due to mode deactivation.");
        }
    }

    void Update()
    {
        if (started)
        {
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                Debug.Log("2 seconds passed (manual)");
                started = false;

                TryAutoConnect("Positive", "ConnPoint");

                // TryAutoConnect("ConnPoint3", "ConnPoint");

                // TryAutoConnect("ConnPoint3", "ConnPoint2");
                // TryAutoConnect("ConnPoint2", "ConnPoint1");
                // TryAutoConnect("ConnPoint2", "ConnPoint");
                // TryAutoConnect("ConnPoint1", "Negative");
                TryAutoConnect("ConnPoint", "Negative");

                // TryAutoConnect("ConnPoint4", "Negative");
                // TryAutoConnect("ConnPoint4", "ConnPoint6");
            }
        }

        if (!connectModeActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.tag.EndsWith("Point"))
                {
                    if (firstPoint == null)
                        SelectFirstPoint(hit.collider.gameObject);
                    else
                        TryConnectPoints(hit.collider.gameObject);
                }
                else
                {
                    Debug.Log("Hit non-ConnectionPoint object: " + hit.collider.name);
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && firstPoint != null)
        {
            Debug.Log("Resetting first point selection: " + firstPoint.name);
            ResetSelection();
        }
    }

    private void SelectFirstPoint(GameObject point)
    {
        firstPoint = point;
        firstPointRenderer = point.GetComponent<Renderer>();

        if (firstPointRenderer != null)
        {
            originalMaterial = firstPointRenderer.material;
            firstPointRenderer.material = backlightMaterial;
        }
        Debug.Log("First point selected: " + point.name);
    }

    private void TryConnectPoints(GameObject secondPoint)
    {
        if (secondPoint == firstPoint)
        {
            Debug.Log("Same point clicked twice. Ignoring.");
            return;
        }

        Transform a = firstPoint.transform;
        Transform b = secondPoint.transform;

        if (ConnectionExists(a, b))
        {
            Debug.Log($"Connection already exists between: {a.name} and {b.name}");
        }
        else
        {
            Debug.Log($"Creating connection between: {a.name} and {b.name}");
            CreateLine(a, b);
            RegisterConnection(a, b);
        }

        ResetSelection();
    }

    private void SetupLineRenderer(LineRenderer lr, Vector3 a, Vector3 b)
    {
        lr.positionCount = 2;
        lr.SetPosition(0, a);
        lr.SetPosition(1, b);
    }

public void SetAllLineMaterials(Material material)
{
    foreach (var lineObj in activeLines.Values)
    {
        if (lineObj != null)
        {
            var lr = lineObj.GetComponent<LineRenderer>();
            if (lr != null)
            {
                lr.material = material;
                lr.startColor = material.color;
                lr.endColor = material.color;
            }
        }
    }

    Debug.Log($"All line materials set to: {material.name}");
}


    void CreateLine(Transform pointA, Transform pointB)
    {
        GameObject lineObj;

        if (lines_buffer.Count > 0)
        {
            lineObj = lines_buffer[^1];
            lines_buffer.RemoveAt(lines_buffer.Count - 1);

            lineObj.SetActive(true);
            LineRenderer lr = lineObj.GetComponent<LineRenderer>();
            SetupLineRenderer(lr, pointA.position, pointB.position);
        }
        else
        {
            lineObj = new GameObject("Line");
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = new Material(lineMaterial);
            SetupLineRenderer(lr, pointA.position, pointB.position);
        }

        LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();
        lineRenderer.startColor = lineMaterial.color;
        lineRenderer.endColor = lineMaterial.color;
        lineRenderer.material.color = lineMaterial.color;

        LineFollow lineFollow = lineObj.GetComponent<LineFollow>() ?? lineObj.AddComponent<LineFollow>();
        lineFollow.Initialize(pointA, pointB);

        activeLines[(pointA, pointB)] = lineObj;
        activeLines[(pointB, pointA)] = lineObj;
    }

void RegisterConnection(Transform a, Transform b)
{
    if (!connectionMap.ContainsKey(a))
        connectionMap[a] = new List<Transform>();

    if (!connectionMap.ContainsKey(b))
        connectionMap[b] = new List<Transform>();

    if (!connectionMap[a].Contains(b))
        connectionMap[a].Add(b);

    if (!connectionMap[b].Contains(a))
        connectionMap[b].Add(a);
}

    public void RemoveConnection(Transform a, Transform b)
    {
        if (connectionMap.ContainsKey(a))
        {
            connectionMap[a].Remove(b);
            if (connectionMap[a].Count == 0)
                connectionMap.Remove(a);
        }

        if (connectionMap.ContainsKey(b))
        {
            connectionMap[b].Remove(a);
            if (connectionMap[b].Count == 0)
                connectionMap.Remove(b);
        }

        if (activeLines.TryGetValue((a, b), out GameObject lineObj))
        {
            lineObj.SetActive(false);
            lines_buffer.Add(lineObj);
            activeLines.Remove((a, b));
            activeLines.Remove((b, a));
        }
    }

    bool ConnectionExists(Transform a, Transform b)
    {
        return connectionMap.ContainsKey(a) && connectionMap[a].Contains(b);
    }

    public List<Transform> GetConnectedPoints(Transform point)
    {
        return connectionMap.ContainsKey(point) ? connectionMap[point] : new List<Transform>();
    }
}
