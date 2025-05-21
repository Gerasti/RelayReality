using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RadiusUIActivator : MonoBehaviour
{
    public SphereCollider detectionCollider;
    public Transform source;              // Камера или игрок
    public float rotationSpeed = 5f;
    public float maxVerticalAngle = 30f;

    private void Awake()
    {
        detectionCollider.enabled = false;

        // Отключаем все DataPanel при старте
        DisableAllDataPanels();
    }

    public void ToggleDetection()
    {
        bool newState = !detectionCollider.enabled;
        detectionCollider.enabled = newState;

        if (newState)
        {
            CheckInitialOverlaps(); // вручную проверяем все элементы в радиусе
        }
        else
        {
            DisableAllDataPanels();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Element"))
        {
            Transform panel = FindChildWithTag(other.transform, "DataPanel");
            if (panel != null)
                panel.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Element"))
        {
            Transform panel = FindChildWithTag(other.transform, "DataPanel");
            if (panel != null)
                panel.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!detectionCollider.enabled) return;

        GameObject[] panels = GameObject.FindGameObjectsWithTag("DataPanel");
        foreach (var panelObj in panels)
        {
            if (panelObj.activeSelf)
                SmoothLookAt(panelObj.transform, source);
        }
    }

void SmoothLookAt(Transform target, Transform lookAt)
{
    Vector3 direction = lookAt.position - target.position;

    if (direction.sqrMagnitude < 0.001f)
        return;

    Vector3 flatDir = new Vector3(direction.x, 0f, direction.z).normalized;
    float verticalAngle = Vector3.Angle(direction, flatDir);
    float sign = Mathf.Sign(direction.y);
    float limitedAngle = Mathf.Min(verticalAngle, maxVerticalAngle);
    Vector3 limitedDir = Quaternion.AngleAxis(limitedAngle * sign, Vector3.Cross(flatDir, Vector3.up)) * flatDir;

    if (limitedDir.sqrMagnitude < 0.001f)
        return;

    // Добавим поправку на локальную ориентацию
    Quaternion targetRotation = Quaternion.LookRotation(limitedDir);
    Quaternion correctedRotation = targetRotation * Quaternion.Euler(0f, 180f, 0f); // Поворачиваем на 180° вокруг Y

    target.rotation = Quaternion.Lerp(target.rotation, correctedRotation, Time.deltaTime * rotationSpeed);
}


    Transform FindChildWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
                return child;
        }
        return null;
    }

    void DisableAllDataPanels()
    {
        GameObject[] allPanels = GameObject.FindGameObjectsWithTag("DataPanel");
        foreach (var panel in allPanels)
            panel.SetActive(false);
    }

    void CheckInitialOverlaps()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionCollider.radius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Element"))
            {
                Transform panel = FindChildWithTag(hit.transform, "DataPanel");
                if (panel != null)
                    panel.gameObject.SetActive(true);
            }
        }
    }
}
