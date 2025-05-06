using UnityEngine;
using System.Collections.Generic;
using System.Linq; 

public enum PowerType { Positive, Negative }

public class PowerPoint : MonoBehaviour
{
    public PowerType type;
    private ConnectionData connectionData;
    
    private Dictionary<Transform, int> levelMap = new();
    private Dictionary<Transform, List<Transform>> previousConnectionMap;

    private ConnectionManager connectionManager;
    private Dictionary<Transform, List<Transform>> connectionMap;

    private void Start()
    {
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

        connectionMap = connectionManager.connectionMap;
        previousConnectionMap = new Dictionary<Transform, List<Transform>>();
    }

private void FixedUpdate()
    {
        if (connectionMap != null && HasConnectionsChanged())
        {
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
            Debug.Log("Проверка PowerPoint не пройдена.");
            return;
        }
        SetVoltageValues();
}
private bool ValidatePowerPoints()
    {
        if (levelMap.Count == 0)
            return false;

        // Находим первый и последний уровни
        int minLevel = levelMap.Values.Min();
        int maxLevel = levelMap.Values.Max();

        var firstLevelTransforms = levelMap.Where(kvp => kvp.Value == minLevel).Select(kvp => kvp.Key).ToList();
        var lastLevelTransforms = levelMap.Where(kvp => kvp.Value == maxLevel).Select(kvp => kvp.Key).ToList();

        // Проверяем, что на первом и последнем уровнях есть объекты с тегом PowerPoint
        var firstPowerPoint = firstLevelTransforms.FirstOrDefault(t => t.CompareTag("PowerPoint"));
        var lastPowerPoint = lastLevelTransforms.FirstOrDefault(t => t.CompareTag("PowerPoint"));

        if (firstPowerPoint == null || lastPowerPoint == null)
        {
            Debug.Log("На первом или последнем уровне отсутствует PowerPoint.");
            return false;
        }

        // Проверяем PowerType
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

        return isValid;
    }

    private void SetVoltageValues()
    {
        int minLevel = levelMap.Values.Min();
        int maxLevel = levelMap.Values.Max();

        // Находим PowerPoint с Positive
        var positivePowerPoint = levelMap
            .Where(kvp => kvp.Key.CompareTag("PowerPoint") && kvp.Key.GetComponent<PowerPoint>().type == PowerType.Positive)
            .Select(kvp => kvp.Key)
            .FirstOrDefault();

        if (positivePowerPoint == null)
        {
            Debug.Log("PowerPoint с Positive не найден.");
            return;
        }

        var positivePowerPointScript = positivePowerPoint.GetComponent<PowerPoint>();
        bool isPositiveAtTop = levelMap[positivePowerPoint] == minLevel;

        // Получаем значение U из ConnectionData объекта с Positive
        float uValue = positivePowerPointScript.connectionData.U;

        Debug.Log($"Positive {(isPositiveAtTop ? "сверху" : "снизу")}, U = {uValue}");

        // Устанавливаем U всем объектам в levelMap
        foreach (var transform in levelMap.Keys)
        {
            var powerPoint = transform.GetComponent<PowerPoint>();
            if (powerPoint != null && powerPoint.connectionData != null)
            {
                powerPoint.connectionData.U = uValue;
                Debug.Log($"Установлено U = {uValue} для {transform.name}");
            }
            else
            {
                Debug.Log($"PowerPoint или ConnectionData отсутствует для {transform.name}");
            }
        }
    }
}
