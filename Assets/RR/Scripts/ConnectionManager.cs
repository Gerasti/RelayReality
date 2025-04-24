using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using System.Linq; 
public class ConnectionManager : MonoBehaviour
{
    public Material lineMaterial;
    public float lineWidth = 0.02f;

    private int connectionCount = 0;
    public float colorDarkenFactor = 0.3f;


    private GameObject firstPoint;
    private bool connectModeActive = false;
    private bool isChangedChain = false;

    private Dictionary<Transform, List<Transform>> connectionMap = new();
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
                if (hit.collider.CompareTag("ConnectionPoint"))
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



private void UnificationU()
{
    isChangedChain = false;
    if (connectionMap.Count == 0)
    {
        Debug.Log("UnificationU: No connections in map");
        return;
    }

    float maxU = 0f;
    HashSet<Transform> visited = new HashSet<Transform>();
    Debug.Log("=== Starting UnificationU ===");

    foreach (var startPoint in connectionMap.Keys)
    {
        if (visited.Contains(startPoint)) continue;

        List<Transform> connectedComponent = GetConnectedComponent(startPoint);
        
        var pointsByLevel = connectedComponent
            .Select(p => (point: p, level: GetComponentLevel(p, startPoint)))
            .GroupBy(x => x.level)
            .OrderBy(g => g.Key);

        // First pass - find max voltage
        foreach (var levelGroup in pointsByLevel)
        {
            Debug.Log($"\nProcessing level {levelGroup.Key} for max voltage:");
            foreach (var (point, _) in levelGroup)
            {
                var pointConnectionData = point.GetComponent<ConnectionData>();
                if (pointConnectionData != null)
                {
                    float oldMaxU = maxU;
                    maxU = Mathf.Max(maxU, pointConnectionData.U);
                    if (maxU > oldMaxU)
                    {
                        Debug.Log($"New max voltage found: {maxU} in point {point.name} at level {levelGroup.Key}");
                    }
                }
            }
        }

        // Second pass - propagate max voltage if it was increased
        if (maxU > 0)        if (maxU > 0)
        {
            Debug.Log($"\nPropagating max voltage {maxU} through levels:");
            foreach (var levelGroup in pointsByLevel)
            {
                Debug.Log($"\nSetting voltage for level {levelGroup.Key}:");
                foreach (var (point, _) in levelGroup)
                {
                    var pointConnectionData = point.GetComponent<ConnectionData>();
                    if (pointConnectionData != null && pointConnectionData.U != maxU)
                    {
                        float oldU = pointConnectionData.U;
                        pointConnectionData.U = maxU;
                        Debug.Log($"Updated voltage in {point.name}: {oldU} -> {maxU} at level {levelGroup.Key}");
                    }
                }
            }
        }

        foreach (var point in connectedComponent)
        {
            visited.Add(point);
        }
    }

    Debug.Log("=== UnificationU Complete ===\n");
}

private List<Transform> GetConnectedComponent(Transform startPoint)
{
    List<Transform> component = new List<Transform>();
    Dictionary<Transform, int> levelMap = new Dictionary<Transform, int>();
    Queue<(Transform point, int level)> queue = new Queue<(Transform, int)>();

    // Start with level 0
    queue.Enqueue((startPoint, 0));
    levelMap[startPoint] = 0;

    Debug.Log($"\n--- Starting component search from {startPoint.name} (Level 0) ---");

    while (queue.Count > 0)
    {
        var (current, level) = queue.Dequeue();
        component.Add(current);
        var currentData = current.GetComponent<ConnectionData>();
        if (currentData != null)
        {
            Debug.Log($"Added to component: {current.name} (Level {level}, U={currentData.U})");
        }

        foreach (var neighbor in GetConnectedPoints(current))
        {
            if (!levelMap.ContainsKey(neighbor))
            {
                int newLevel = level + 1;
                levelMap[neighbor] = newLevel;
                queue.Enqueue((neighbor, newLevel));
                
                var neighborData = neighbor.GetComponent<ConnectionData>();
                if (neighborData != null)
                {
                    Debug.Log($"Found new connection: {current.name}(L{level},U={currentData?.U}) -> {neighbor.name}(L{newLevel},U={neighborData.U})");
                }
            }
        }
    }

    // Print hierarchy
    PrintHierarchy(startPoint, levelMap, "");

    return component;
}

private void PrintHierarchy(Transform point, Dictionary<Transform, int> levelMap, string indent)
{
    var data = point.GetComponent<ConnectionData>();
    Debug.Log($"{indent}Level {levelMap[point]}: {point.name} (U={data?.U})");
    
    foreach (var neighbor in GetConnectedPoints(point))
    {
        if (levelMap[neighbor] > levelMap[point])
        {
            PrintHierarchy(neighbor, levelMap, indent + "  ");
        }
    }
}

private int GetComponentLevel(Transform point, Transform root)
{
    int level = 0;
    Queue<(Transform p, int l)> queue = new Queue<(Transform, int)>();
    queue.Enqueue((root, 0));
    HashSet<Transform> visited = new HashSet<Transform>();

    while (queue.Count > 0)
    {
        var (current, currentLevel) = queue.Dequeue();
        if (current == point) return currentLevel;

        if (visited.Add(current))
        {
            foreach (var neighbor in GetConnectedPoints(current))
            {
                queue.Enqueue((neighbor, currentLevel + 1));
            }
        }
    }
    return level;
}

void FixedUpdate()
{
    if(isChangedChain){
    Debug.Log("=== FixedUpdate: Starting voltage unification ===");
    UnificationU();
    }
}



}
