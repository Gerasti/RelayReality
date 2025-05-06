using UnityEngine;
using System.Collections.Generic;
public class ConnectionManager : MonoBehaviour
{
    public Material lineMaterial;
    public float lineWidth = 0.02f;

    private int connectionCount = 0;
    public float colorDarkenFactor = 0.3f;


    private GameObject firstPoint;
    private bool connectModeActive = false;
    private bool isChangedChain = false;

    public Dictionary<Transform, List<Transform>> connectionMap = new();
    private List<GameObject> lines_buffer = new();
    private Dictionary<(Transform, Transform), GameObject> activeLines = new();


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

    void Update()
    {
        if (!connectModeActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.tag.EndsWith("Point"))
                {
                    if (firstPoint == null)
                    {
                        SelectFirstPoint(hit.collider.gameObject);
                    }
                    else
                    {
                        TryConnectPoints(hit.collider.gameObject);
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

    private void SelectFirstPoint(GameObject point)
    {
        firstPoint = point;
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
            Debug.Log($"Connection registered between: {a.name} and {b.name}");
        }

        firstPoint = null;
    }

    private void SetupLineRenderer(LineRenderer lr, Vector3 a, Vector3 b)
    {
        lr.positionCount = 2;
        lr.SetPosition(0, a);
        lr.SetPosition(1, b);
    }


        void CreateLine(Transform pointA, Transform pointB)
    {
        GameObject lineObj;
        isChangedChain = true;

        if (lines_buffer.Count > 0)
        {
            lineObj = lines_buffer[lines_buffer.Count - 1];
            lines_buffer.RemoveAt(lines_buffer.Count - 1);
            
            lineObj.SetActive(true);
            LineRenderer lineR = lineObj.GetComponent<LineRenderer>();
            
            SetupLineRenderer(lineR, pointA.position, pointB.position);

        }
        else
        {
            lineObj = new GameObject("Line");
            LineRenderer lineR = lineObj.AddComponent<LineRenderer>();
            lineR.material = new Material(lineMaterial);
            lineR.startWidth = lineWidth;
            lineR.endWidth = lineWidth;
            
            SetupLineRenderer(lineR, pointA.position, pointB.position);

        }


        LineRenderer lr = lineObj.GetComponent<LineRenderer>();
        Color baseColor = lineMaterial.color;
        float intensity = Mathf.Pow(colorDarkenFactor, connectionCount);
        Color newColor = new Color(baseColor.r * intensity, baseColor.g * intensity, baseColor.b * intensity);
        lr.startColor = newColor;
        lr.endColor = newColor;
        lr.material.color = newColor;

        connectionCount++;

LineFollow lineFollow = lineObj.GetComponent<LineFollow>() ?? lineObj.AddComponent<LineFollow>();
lineFollow.Initialize(pointA, pointB);


        activeLines[(pointA, pointB)] = lineObj;
        activeLines[(pointB, pointA)] = lineObj;
    }

    void RegisterConnection(Transform a, Transform b)
    {
        if (!connectionMap.ContainsKey(a))
            connectionMap[a] = new List<Transform>();
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

        if (activeLines.TryGetValue((a, b), out GameObject lineObj))
        {
            lineObj.SetActive(false);
            lines_buffer.Add(lineObj);
            activeLines.Remove((a, b));
            activeLines.Remove((b, a));

            isChangedChain = true;
            Debug.Log($"Line deactivated and added to buffer between {a.name} and {b.name}");
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
