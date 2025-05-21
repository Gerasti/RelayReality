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

    private Dictionary<Transform, int> levelMap = new();
    private Dictionary<Transform, List<Transform>> previousConnectionMap;

    private ConnectionManager connectionManager;
    private Dictionary<Transform, List<Transform>> connectionMap;
    private Dictionary<(Transform from, Transform to), float> lineAnimationOffsets = new();
    private HashSet<(Transform from, Transform to)> activeAnimatedLines = new();
    private bool isAnimationActive = false;



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
        connectionData.dataChanged = false;
        TraceLevels(transform);
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
                lineAnimationOffsets[key] = Random.Range(0f, Mathf.PI * 2); // случайный сдвиг

            float offset = lineAnimationOffsets[key];
            float pulse = Mathf.Abs(Mathf.Sin(time * 2f + offset)); // скорость пульсации

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

            return;
        }



        isAnimationActive = true;
        connectionManager.SetPulseActiveForAll(true);
        SetVoltageValues();



    }


    private bool ValidatePowerPoints()
    {
        if (levelMap.Count == 0)
            return false;

        int minLevel = levelMap.Values.Min();
        int maxLevel = levelMap.Values.Max();

        var firstLevelTransforms = levelMap.Where(kvp => kvp.Value == minLevel).Select(kvp => kvp.Key).ToList();
        var lastLevelTransforms = levelMap.Where(kvp => kvp.Value == maxLevel).Select(kvp => kvp.Key).ToList();

        var firstPowerPoint = firstLevelTransforms.FirstOrDefault(t => t.CompareTag("PowerPoint"));
        var lastPowerPoint = lastLevelTransforms.FirstOrDefault(t => t.CompareTag("PowerPoint"));

        if (firstPowerPoint == null || lastPowerPoint == null)
        {
            Debug.Log("На первом или последнем уровне отсутствует PowerPoint.");
            return false;
        }


        var firstPowerPointScript = firstPowerPoint.GetComponent<PowerPoint>();
        var lastPowerPointScript = lastPowerPoint.GetComponent<PowerPoint>();

        if (firstPowerPointScript == null || lastPowerPointScript == null)
        {
            Debug.Log("Компонент PowerPoint отсутствует на объектах.");
            return false;
        }

        bool isValid = (firstPowerPointScript.type == PowerType.Positive && lastPowerPointScript.type == PowerType.Negative) ||
                       (firstPowerPointScript.type == PowerType.Negative && lastPowerPointScript.type == PowerType.Positive);

        if (!isValid)
        {
            Debug.Log("Недопустимая комбинация PowerType: должен быть Positive и Negative на разных концах.");
        }

        foreach (var node in levelMap.Keys)
        {
            if (!connectionMap.TryGetValue(node, out var connections))
            {
                Debug.Log($"Узел {node.name} не найден в connectionMap.");
                return false;
            }

            int connectionCount = connections.Count;

            // PowerPoint может иметь только одно соединение
            if (node.CompareTag("PowerPoint"))
            {
                if (connectionCount < 1)
                {
                    Debug.Log($"PowerPoint {node.name} должен иметь хотя бы одно соединение.");
                    return false;
                }
            }
            else
            {
                if (connectionCount < 2)
                {
                    Debug.Log($"Узел {node.name} должен иметь как минимум два соединения.");
                    return false;
                }
            }
        }


        return isValid;
    }

    private void SetVoltageValues()
    {
        int minLevel = levelMap.Values.Min();
        int maxLevel = levelMap.Values.Max();

        var positivePowerPoint = levelMap
            .Where(kvp => kvp.Key.CompareTag("PowerPoint") && kvp.Key.GetComponent<PowerPoint>().type == PowerType.Positive)
            .Select(kvp => kvp.Key)
            .FirstOrDefault();

        var negativePowerPoint = levelMap
            .Where(kvp => kvp.Key.CompareTag("PowerPoint") && kvp.Key.GetComponent<PowerPoint>().type == PowerType.Negative)
            .Select(kvp => kvp.Key)
            .FirstOrDefault();

        if (positivePowerPoint == null || negativePowerPoint == null)
        {
            Debug.Log("PowerPoints not found.");
            return;
        }

        var positivePowerPointScript = positivePowerPoint.GetComponent<PowerPoint>();
        var negativePowerPointScript = negativePowerPoint.GetComponent<PowerPoint>();

        bool isPositiveAtTop = levelMap[positivePowerPoint] == minLevel;
        float uValue = positivePowerPointScript.connectionData.U;
        float iValue = positivePowerPointScript.connectionData.I;

        Debug.Log($"Positive is at {(isPositiveAtTop ? "top" : "bottom")}, U = {uValue}");

        // Очистка старых активных линий
        activeAnimatedLines.Clear();

        foreach (var kvp in levelMap)
        {
            Transform from = kvp.Key;

            if (!connectionMap.TryGetValue(from, out var neighbors))
                continue;

            foreach (var to in neighbors)
            {
                if (!levelMap.ContainsKey(to) || levelMap[to] <= levelMap[from])
                    continue;

                if (connectionManager.activeLines.TryGetValue((from, to), out GameObject lineObj))
                {
                    var lr = lineObj.GetComponent<LineRenderer>();
                    if (lr != null)
                    {
                        activeAnimatedLines.Add((from, to));
                        Debug.Log($"Set animated color for line between {from.name} and {to.name}.");
                    }
                }
            }
        }


        float totalResistance = CalculateTotalResistance(positivePowerPoint, negativePowerPoint, new HashSet<Transform>());

        if (totalResistance == 0)
        {
            Debug.Log("⚠️ Общее сопротивление равно 0, невозможен расчёт тока.");
            return;
        }


        float calculatedCurrent = uValue / totalResistance;
        Debug.Log($"[Ohm] Общее сопротивление: {totalResistance:F3}, Расчётный ток I = {calculatedCurrent:F3}");


        positivePowerPointScript.connectionData.I = calculatedCurrent;
        negativePowerPointScript.connectionData.I = calculatedCurrent;


        float declaredI = positivePowerPointScript.connectionData.I;

        if (Mathf.Abs(declaredI - calculatedCurrent) < 0.01f)
            Debug.Log("✅ Закон Ома выполняется.");
        else
            Debug.Log($"ℹ️ Заданный ток I = {declaredI:F3}, но по расчёту I = {calculatedCurrent:F3}. Обновлён.");

CalculateCurrentsForEachElement(positivePowerPoint);


    }

private void CalculateCurrentsForEachElement(Transform positivePoint)
{
    Dictionary<Transform, float> resistanceToElement = new();
    resistanceToElement[positivePoint] = 0f;

    int maxLevel = levelMap.Values.Max();

    for (int level = levelMap[positivePoint] + 1; level <= maxLevel; level++)
    {
        var elementsAtLevel = levelMap.Where(kvp => kvp.Value == level).Select(kvp => kvp.Key).ToList();

        foreach (var current in elementsAtLevel)
        {
            if (!connectionMap.TryGetValue(current, out var prevList))
                continue;

            // Найти предыдущие узлы с уровнем меньше
            var incoming = prevList.Where(p => levelMap.ContainsKey(p) && levelMap[p] < level).ToList();

            if (incoming.Count == 0)
                continue;

            List<float> pathsResistance = new();

            foreach (var from in incoming)
            {
                if (!resistanceToElement.TryGetValue(from, out float rToPrev))
                    continue;

                float ownR = current.GetComponent<ConnectionData>()?.R ?? 0f;
                pathsResistance.Add(rToPrev + ownR);
            }

            float totalResistance;
            if (pathsResistance.Count == 1)
            {
                totalResistance = pathsResistance[0]; // Последовательно
            }
            else
            {
                totalResistance = 1f / pathsResistance.Sum(r => 1f / r); // Параллельно
            }

            resistanceToElement[current] = totalResistance;

            // Вычисляем ток: I = U / R
            float voltage = positivePoint.GetComponent<ConnectionData>().U;
            float currentI = totalResistance > 0 ? voltage / totalResistance : 0f;
            current.GetComponent<ConnectionData>().I = currentI;

            Debug.Log($"[Element {current.name}] Resistance to here: {totalResistance:F3}, Current I = {currentI:F3}");
        }
    }
}


    private float CalculateTotalResistance(Transform current, Transform target, HashSet<Transform> visited)
    {
        if (current == target)
            return 0f;

        visited.Add(current);

        if (!connectionMap.TryGetValue(current, out var neighbors))
            return float.PositiveInfinity;

        List<float> resistances = new();

        foreach (var next in neighbors)
        {
            if (visited.Contains(next))
                continue;

            float ownResistance = next.GetComponent<ConnectionData>()?.R ?? 0f;
            float nextResistance = CalculateTotalResistance(next, target, new HashSet<Transform>(visited));

            if (!float.IsInfinity(nextResistance))
                resistances.Add(ownResistance + nextResistance);
        }

        if (resistances.Count == 0)
            return float.PositiveInfinity;
        else if (resistances.Count == 1)
            return resistances[0];
        else
            return 1f / resistances.Sum(r => 1f / r);
    }


}


