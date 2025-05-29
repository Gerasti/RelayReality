using UnityEngine;
using System.Collections.Generic;
using System.Linq;



public enum PowerType { Positive, Negative }

public class PowerPoint : MonoBehaviour
{
    private Renderer rend;
    private Material originalMaterial;
    public Material activeMaterial;
    public PowerType type;
    private ConnectionData connectionData;
    private Transform positivePoint;
    private Transform negativePoint;


    private Dictionary<Transform, int> levelMap = new();
    private Dictionary<Transform, List<Transform>> previousConnectionMap;

    private ConnectionManager connectionManager;
    private Dictionary<Transform, List<Transform>> connectionMap;
    private Dictionary<(Transform from, Transform to), float> lineAnimationOffsets = new();
    private HashSet<(Transform from, Transform to)> activeAnimatedLines = new();
    private bool isAnimationActive = false;

    private void Awake()
    {
        ConnectionData.OnAnyDataChanged += RecalculateNetwork;
    }

    private void Start()
    {
        ConnectionData.OnAnyDataChanged += () => connectionData.dataChanged = true;

        connectionManager = FindObjectOfType<ConnectionManager>();
        if (connectionManager == null)
        {
            Debug.Log("ConnectionManager not found in the scene!");
            return;
        }
        connectionData = GetComponent<ConnectionData>();
        if (connectionData == null)
        {
            Debug.Log($"ConnectionData component missing on {gameObject.name}");
            return;
        }
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalMaterial = rend.material;
        }

        connectionMap = connectionManager.connectionMap;
        previousConnectionMap = new Dictionary<Transform, List<Transform>>();
    }

    private void FixedUpdate()
    {
        if (connectionMap == null) return;

        if (HasConnectionsChanged() || connectionData.dataChanged)
        {
            TraceLevels(transform);
            connectionData.dataChanged = false;
        }
    }

    private bool HasConnectionsChanged()
    {
        if (connectionMap.Count != previousConnectionMap.Count)
        {
            UpdatePreviousConnectionMap();
            return true;
        }

        foreach (var pair in connectionMap)
        {
            if (!previousConnectionMap.TryGetValue(pair.Key, out var prevNeighbors) ||
                !AreListsEqual(pair.Value, prevNeighbors))
            {
                UpdatePreviousConnectionMap();
                return true;
            }
        }

        return false;
    }

    private void Update()
    {
        if (isAnimationActive)
        {
            AnimateLines();
        }
    }


    private void AnimateLines()
    {
        if (connectionManager == null || connectionManager.activeLines == null)
            return;

        float time = Time.time;

        foreach (var key in activeAnimatedLines)
        {
            if (!connectionManager.activeLines.TryGetValue(key, out GameObject lineObj))
                continue;

            var lr = lineObj.GetComponent<LineRenderer>();
            if (lr == null)
                continue;

            if (!lineAnimationOffsets.ContainsKey(key))
                lineAnimationOffsets[key] = Random.Range(0f, Mathf.PI * 2); 

            float offset = lineAnimationOffsets[key];
            float pulse = Mathf.Abs(Mathf.Sin(time * 2f + offset));

            Color baseColor = activeMaterial.color;
            Color animatedColor = Color.Lerp(Color.black, baseColor, pulse);

            lr.startColor = animatedColor;
            lr.endColor = animatedColor;

            lr.material.color = animatedColor;

        }
    }



    private void UpdatePreviousConnectionMap()
    {
        previousConnectionMap.Clear();
        foreach (var pair in connectionMap)
        {
            previousConnectionMap[pair.Key] = new List<Transform>(pair.Value);
        }
    }
    private bool AreListsEqual(List<Transform> list1, List<Transform> list2)
    {
        if (list1 == null || list2 == null || list1.Count != list2.Count)
            return false;

        return list1.TrueForAll(t => list2.Contains(t)) && list2.TrueForAll(t => list1.Contains(t));
    }

    public void ResetMaterial()
    {
        if (rend != null && originalMaterial != null)
        {
            rend.material = originalMaterial;
        }
    }

    private void TraceLevels(Transform start)
    {
        levelMap.Clear();
        var visited = new HashSet<Transform>();
        var queue = new Queue<Transform>();

        queue.Enqueue(start);
        visited.Add(start);
        levelMap[start] = 0;

        while (queue.Count > 0)
        {
            Transform current = queue.Dequeue();
            int currentLevel = levelMap[current];

            Debug.Log($"Level {currentLevel}: {current.name}");

            if (!connectionMap.TryGetValue(current, out var neighbors))
                continue;

            foreach (var neighbor in neighbors)
            {
                if (visited.Contains(neighbor))
                    continue;

                visited.Add(neighbor);
                queue.Enqueue(neighbor);
                levelMap[neighbor] = currentLevel + 1;
            }
        }
        if (!ValidatePowerPoints())
        {
            Debug.Log("PowerPoint validation failed.");

            isAnimationActive = false;

            connectionManager.SetPulseActiveForAll(false);
            connectionManager.SetAllLineMaterials(connectionManager.lineMaterial);

            foreach (var t in levelMap.Keys)
            {
                var p = t.GetComponent<PowerPoint>();
                if (p != null)
                    p.ResetMaterial();

            }

            foreach (var kvp in connectionMap)
            {
                var data = kvp.Key.GetComponent<ConnectionData>();
                if (data != null)
                    data.I = 0f;
            }

            return;
        }

        activeAnimatedLines.Clear();
        foreach (var from in connectionMap.Keys)
        {
            foreach (var to in connectionMap[from])
            {
                if (levelMap[to] > levelMap[from])
                    activeAnimatedLines.Add((from, to));
            }
        }



        isAnimationActive = true;
        connectionManager.SetPulseActiveForAll(true);

        SetVoltageValues();

    }


    private bool ValidatePowerPoints()
    {
        positivePoint = null;
        negativePoint = null;

        foreach (var kvp in levelMap)
        {
            var t = kvp.Key;
            var power = t.GetComponent<PowerPoint>();
            connectionMap.TryGetValue(t, out var neighbors);
            int connectionCount = neighbors?.Count ?? 0;

            if (power != null)
            {
                if (power.type == PowerType.Positive)
                {
                    positivePoint = t;
                    if (connectionCount < 1)
                    {
                        Debug.LogWarning("Positive point must have at least one connection.");
                        return false;
                    }
                }
                else if (power.type == PowerType.Negative)
                {
                    negativePoint = t;
                    if (connectionCount < 1)
                    {
                        Debug.LogWarning("Negative point must have at least one connection.");
                        return false;
                    }
                }
            }
            else
            {
                if (connectionCount < 2)
                {
                    Debug.LogWarning($"Element {t.name} must have at least two connections.");
                    return false;
                }
            }
        }

        if (positivePoint == null || negativePoint == null)
        {
            Debug.LogWarning("Positive or Negative PowerPoint not found in chain.");
            return false;
        }

        int posLevel = levelMap[positivePoint];
        int negLevel = levelMap[negativePoint];

        if (posLevel > negLevel)
        {
            InvertLevelMap();
        }

        return true;
    }

    private void InvertLevelMap()
    {
        int maxLevel = levelMap.Values.Max();
        var inverted = levelMap.ToDictionary(kvp => kvp.Key, kvp => maxLevel - kvp.Value);
        levelMap = inverted;
    }



    private void SetVoltageValues()
    {
        if (positivePoint == null)
        {
            Debug.LogError("Positive point is null. Voltage calculation aborted.");
            return;
        }

        RecalculateNetwork();
        ValidateKirchhoffCurrentLaw();
    }

    public void RecalculateNetwork()
    {
        if (negativePoint == null)
        {
            return;
        }

        if (!connectionMap.ContainsKey(positivePoint) || !connectionMap.ContainsKey(negativePoint))
            return;

        List<Transform> nodes = new(connectionMap.Keys);
        int N = nodes.Count;
        var nodeIndices = new Dictionary<Transform, int>();
        for (int i = 0; i < N; i++)
            nodeIndices[nodes[i]] = i;

        float[,] G = new float[N, N];
        float[] I = new float[N];

    foreach(var from in connectionMap)
    {
        foreach(var to in from.Value)
        {
            if(!nodeIndices.ContainsKey(from.Key) || !nodeIndices.ContainsKey(to))
                continue;

            int i = nodeIndices[from.Key];
            int j = nodeIndices[to];

            float r = CalculateEffectiveResistance(from.Key, to);
            
            if(r < 0.0001f)
            {
                Debug.LogWarning($"Very small resistance from {from.Key.name} to {to.name} (r = {r}). Skipping.");
                continue;
            }

            float g = 1f / r;

            G[i, i] += g;
            G[j, j] += g;
            G[i, j] -= g;
            G[j, i] -= g;
        }
    }

        int refIndex = nodeIndices[negativePoint];
        for (int k = 0; k < N; k++)
            G[refIndex, k] = 0f;
        G[refIndex, refIndex] = 1f;
        I[refIndex] = 0f;

        int posIndex = nodeIndices[positivePoint];
        float voltage = positivePoint.GetComponent<ConnectionData>().U;

        for (int k = 0; k < N; k++)
            G[posIndex, k] = 0f;
        G[posIndex, posIndex] = 1f;
        I[posIndex] = voltage;

        float[] potentials = SolveLinearSystem(G, I);

        for (int i = 0; i < N; i++)
        {
            var node = nodes[i];
            var data = node.GetComponent<ConnectionData>();
            if (data != null)
            {
                data.U = potentials[i];
                Debug.Log($"[Voltage] {node.name}: U = {data.U:F3} В");
            }
        }

        RecalculateCurrents();
    }



private void RecalculateCurrents()
{
    HashSet<string> processedConnections = new();

    foreach(var from in connectionMap)
    {
        var fromData = from.Key.GetComponent<ConnectionData>();
        if(fromData == null) continue;

            foreach (var to in from.Value)
            {

                var toData = to.GetComponent<ConnectionData>();
                if (toData == null) continue;



                string connectionKey = GetConnectionKey(from.Key, to);
                if (processedConnections.Contains(connectionKey))
                    continue;

                processedConnections.Add(connectionKey);

                ConnectionType connType = GetConnectionType(from.Key, to);
                float rFrom = fromData.R;
                float rTo = toData.R;
                float totalR;

                if (connType == ConnectionType.Sequential)
                {
                    totalR = rFrom + rTo;
                }
                else // Parallel
                {
                    totalR = (rFrom * rTo) / (rFrom + rTo);
                }

                float u1 = fromData.U;
                float u2 = toData.U;

                if (totalR < 0.0001f)
                {
                    continue;
                }

                if (u1 <= u2) continue;

                float i = (u1 - u2) / totalR;

                if (connType == ConnectionType.Parallel)
                {
                    fromData.I = i * (rTo / (rFrom + rTo));
                    toData.I = i * (rFrom / (rFrom + rTo));
                }
                else
                {
                    fromData.I = i;
                    toData.I = i;
                }



                Debug.Log($"[{connType}] {from.Key.name} → {to.name}: R={totalR:F3}, I={i:F3}");
                
            
        }
    }
}

    private string GetConnectionKey(Transform a, Transform b)
    {
        int idA = a.GetInstanceID();
        int idB = b.GetInstanceID();
        return idA < idB ? $"{idA}-{idB}" : $"{idB}-{idA}";
    }


    private float[] SolveLinearSystem(float[,] A, float[] b)
    {
        int N = b.Length;
        float[,] M = new float[N, N + 1];

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
                M[i, j] = A[i, j];
            M[i, N] = b[i];
        }

        for (int i = 0; i < N; i++)
        {
            float diag = M[i, i];
            if (Mathf.Abs(diag) < 1e-6f) continue;

            for (int j = 0; j <= N; j++)
                M[i, j] /= diag;

            for (int k = 0; k < N; k++)
            {
                if (k == i) continue;
                float factor = M[k, i];
                for (int j = 0; j <= N; j++)
                    M[k, j] -= factor * M[i, j];
            }
        }

        float[] result = new float[N];
        for (int i = 0; i < N; i++)
            result[i] = M[i, N];

        return result;
    }

    private void ValidateKirchhoffCurrentLaw()
    {
        foreach (var kvp in levelMap)
        {
            var node = kvp.Key;
            var connData = node.GetComponent<ConnectionData>();
            if (connData == null) continue;

            var powerScript = node.GetComponent<PowerPoint>();
            if (powerScript != null && (powerScript.type == PowerType.Negative || powerScript.type == PowerType.Positive))
            {
                continue;
            }

            if (!connectionMap.TryGetValue(node, out var neighbors)) continue;

            float incomingSum = 0f;
            float outgoingSum = 0f;

            foreach (var neighbor in neighbors)
            {
                if (!levelMap.ContainsKey(neighbor)) continue;

                var neighborData = neighbor.GetComponent<ConnectionData>();
                if (neighborData == null) continue;

                if (levelMap[neighbor] < levelMap[node])
                {
                    incomingSum += neighborData.I;
                }
                else if (levelMap[neighbor] > levelMap[node])
                {
                    outgoingSum += neighborData.I;
                }
            }

            float difference = Mathf.Abs(incomingSum - outgoingSum);
            if (difference > 0.01f)
            {
              Debug.LogWarning($"[Kirchhoff] Current imbalance detected at node {node.name}: ∑I_in = {incomingSum:F3}, ∑I_out = {outgoingSum:F3}");
            }
            else
            {
            Debug.Log($"[Kirchhoff] Node {node.name} — law holds: I_in = I_out = {incomingSum:F3}");

            }
        }
    }

private ConnectionType GetConnectionType(Transform from, Transform to)
{
    if (Mathf.Abs(levelMap[from] - levelMap[to]) != 1)
        return ConnectionType.Sequential;

    bool isParallel = CheckParallelByCommonNode(from, to);

    return isParallel ? ConnectionType.Parallel : ConnectionType.Sequential;
}

private bool CheckParallelByCommonNode(Transform from, Transform to)
{
    if (connectionMap[from].Count(n => n != to && levelMap[n] == levelMap[to]) > 0)
    {
        return true;
    }

    if (connectionMap.Keys.Count(n => 
            n != from && 
            levelMap[n] == levelMap[from] && 
            connectionMap[n].Contains(to)) > 0)
    {
        return true;
    }
        return false;
}





private Transform FindCommonHigherNeighbor(Transform nodeA, Transform nodeB)
{
    var higherNeighborsA = connectionMap[nodeA]
        .Where(n => levelMap[n] == levelMap[nodeA] - 1)
        .ToList();

    var higherNeighborsB = connectionMap[nodeB]
        .Where(n => levelMap[n] == levelMap[nodeB] - 1)
        .ToList();

    return higherNeighborsA.Intersect(higherNeighborsB).FirstOrDefault();
}


    private enum ConnectionType { Sequential, Parallel }

private float CalculateEffectiveResistance(Transform from, Transform to)
{
    float rFrom = from.GetComponent<ConnectionData>()?.R ?? 1f;
    float rTo = to.GetComponent<ConnectionData>()?.R ?? 1f;
    
    switch(GetConnectionType(from, to))
    {
        case ConnectionType.Sequential:
            return rFrom + rTo;
            
        case ConnectionType.Parallel:
            return (rFrom * rTo) / (rFrom + rTo);
            
        default:
            return rFrom + rTo;
    }
}

}


 





