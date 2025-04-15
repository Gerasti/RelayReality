using UnityEngine;
using System.Collections.Generic;

public class ConnectionManager : MonoBehaviour
{
    public Material lineMaterial;
    public float lineWidth = 0.02f;

    private int connectionCount = 0; // счётчик соединений
    public float colorDarkenFactor = 0.5f; // фактор затемнения


    private GameObject firstPoint;
    private bool connectModeActive = false;

    private Dictionary<Transform, List<Transform>> connectionMap = new();

    public void SetConnectMode(bool active)
    {
        connectModeActive = active;
        Debug.Log("Connect Mode: " + (active ? "ON" : "OFF"));

        if (!active)
        {
            firstPoint = null;
            Debug.Log("First point reset due to mode deactivation.");
        }
    }

    private bool IsConnectModeActive()
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
                        Debug.Log("First point selected: " + firstPoint.name);
                    }
                    else
                    {
                        GameObject secondPoint = hit.collider.gameObject;

                        if (secondPoint == firstPoint)
                        {
                            Debug.Log("Same point clicked twice. Ignoring.");
                            return;
                        }

                        Transform a = firstPoint.transform;
                        Transform b = secondPoint.transform;

                        if (ConnectionExists(a, b))
                        {
                            Debug.Log("Connection already exists between: " + a.name + " and " + b.name);
                        }
                        else
                        {
                            Debug.Log("Creating connection between: " + a.name + " and " + b.name);
                            CreateLine(a, b);
                            RegisterConnection(a, b);
                            Debug.Log("Connection registered between: " + a.name + " and " + b.name);
                        }

                        firstPoint = null;
                    }
                }
                else
                {
                    Debug.Log("Hit non-ConnectionPoint object: " + hit.collider.name);
                }
            }
            else
            {
                Debug.Log("Raycast did not hit any object.");
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (firstPoint != null)
            {
                Debug.Log("Resetting first point selection: " + firstPoint.name);
                firstPoint = null;
            }
        }
    }

    void CreateLine(Transform pointA, Transform pointB)
{
    GameObject lineObj = new GameObject("Line");
    LineRenderer lr = lineObj.AddComponent<LineRenderer>();
    lr.material = new Material(lineMaterial); // создаём копию, чтобы не менять оригинал
    lr.startWidth = lineWidth;
    lr.endWidth = lineWidth;
    lr.positionCount = 2;

    lr.SetPosition(0, pointA.position);
    lr.SetPosition(1, pointB.position);

    // Затемнение цвета
    Color baseColor = lineMaterial.color;
    float intensity = Mathf.Pow(colorDarkenFactor, connectionCount);
    Color newColor = new Color(baseColor.r * intensity, baseColor.g * intensity, baseColor.b * intensity);
    lr.startColor = newColor;
    lr.endColor = newColor;
    lr.material.color = newColor;

    connectionCount++;

    LineFollow follow = lineObj.AddComponent<LineFollow>();
    follow.pointA = pointA;
    follow.pointB = pointB;

    Debug.Log("LineRenderer created between: " + pointA.name + " and " + pointB.name + " with color: " + newColor);
}

    void RegisterConnection(Transform a, Transform b)
    {
        if (!connectionMap.ContainsKey(a))
            connectionMap[a] = new List<Transform>();
        if (!connectionMap[a].Contains(b))
            connectionMap[a].Add(b);

        if (!connectionMap.ContainsKey(b))
            connectionMap[b] = new List<Transform>();
        if (!connectionMap[b].Contains(a))
            connectionMap[b].Add(a);
    }

    public void RemoveConnection(Transform a, Transform b)
{
    if (connectionMap.ContainsKey(a))
    {
        connectionMap[a].Remove(b);

        if (connectionMap[a].Count == 0)
        {
            connectionMap.Remove(a);
        }
    }

    if (connectionMap.ContainsKey(b))
    {
        connectionMap[b].Remove(a);
        if (connectionMap[b].Count == 0)
        {
            connectionMap.Remove(b);
        }
    }
    Debug.Log($"Connection removed between {a.name} and {b.name}");
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
