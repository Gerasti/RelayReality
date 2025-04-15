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
        UpdateLine();
    }

    void LateUpdate()
{
    Vector3 currentA = pointA.position;
    Vector3 currentB = pointB.position;

    if (currentA != lastA || currentB != lastB)
    {
        // Находим ConnectionManager
        ConnectionManager connectionManager = FindObjectOfType<ConnectionManager>();
        if (connectionManager != null)
        {
            // Удаляем старую связь
            connectionManager.RemoveConnection(pointA, pointB);
        }

        lr.positionCount = 0;
        lastA = currentA;
        lastB = currentB;
    }
}


    void UpdateLine()
    {
        Vector3 posA = pointA.position;
        Vector3 posB = pointB.position;


        // Проверка всех коллизий между A и B
        RaycastHit[] hits = Physics.RaycastAll(posA, (posB - posA).normalized, Vector3.Distance(posA, posB));

        int elementHitCount = 0;
        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Element"))
                elementHitCount++;
        }

        // Если прошли через более чем один элемент — очищаем линию
        if (elementHitCount > 1)
        {
            lr.positionCount = 0;
        }
        else
        {
            lr.positionCount = 2;
            lr.SetPosition(0, posA);
            lr.SetPosition(1, posB);
        }

        lastA = posA;
        lastB = posB;
    }
}
