using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RadiusUIActivator : MonoBehaviour
{
    [Header("Detection")]
    public SphereCollider detectionCollider;
    public Transform source; // Камера или игрок
    public Vector3 centerOffset = Vector3.zero; // Смещение от source.position для центра сферы
    public float rotationSpeed = 5f;
    public float maxVerticalAngle = 30f;
    private HashSet<Transform> lastElementsInRange = new HashSet<Transform>();
    private Dictionary<Transform, Vector3> originalPanelPositions = new Dictionary<Transform, Vector3>();


    [Header("UI")]
    public Color activeColor = Color.green;
    public Color defaultColor = Color.white;

    private bool IsActive = false;
    private Image image;
    private Button button;

    private void Awake()
    {
        detectionCollider.enabled = false;
        DisableAllDataPanels();
    }

    private void Start()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(ToggleDetection);

        if (image != null)
            image.color = defaultColor;
    }

    public void ToggleDetection()
    {
        IsActive = !IsActive;
        detectionCollider.enabled = IsActive;

        if (image != null)
            image.color = IsActive ? activeColor : defaultColor;

        if (!IsActive)
            DisableAllDataPanels();
    }

 private void Update()
{
    if (!IsActive) return;

    float radius = detectionCollider.radius;
    Vector3 center = source.position + centerOffset;

    Collider[] hits = Physics.OverlapSphere(center, radius);
    HashSet<Transform> elementsInRange = new HashSet<Transform>();

    int dataPanelCount = 0;

    foreach (var hit in hits)
    {
        if (hit.CompareTag("Element"))
        {
            elementsInRange.Add(hit.transform);

            Transform panel = FindChildWithTag(hit.transform, "DataPanel");
            if (panel != null)
            {
HoldElements hold = hit.GetComponent<HoldElements>();
if (!originalPanelPositions.ContainsKey(panel))
{
    if (hold != null && hold.IsHeld && panel.parent != null && panel.parent.parent != null)
        originalPanelPositions[panel] = panel.parent.parent.position;
    else if (panel.parent != null)
        originalPanelPositions[panel] = panel.parent.position;
    else
        originalPanelPositions[panel] = panel.position;
}
else
{
    // Обновляем каждый кадр, если объект держат или отпустили
    if (hold != null)
    {
        if (hold.IsHeld && panel.parent != null && panel.parent.parent != null)
            originalPanelPositions[panel] = panel.parent.parent.position;
        else if (!hold.IsHeld && panel.parent != null)
            originalPanelPositions[panel] = panel.parent.position;
    }
}


if (!panel.gameObject.activeSelf)
    panel.gameObject.SetActive(true);

SmoothLookAt(panel, source);
AdjustPanelPosition(panel, originalPanelPositions[panel], source.position, 0.5f);

                dataPanelCount++;
            }
        }
    }

    foreach (Transform element in lastElementsInRange)
    {
        if (!elementsInRange.Contains(element))
        {
            Transform panel = FindChildWithTag(element, "DataPanel");
            if (panel != null)
                panel.gameObject.SetActive(false);
        }
    }

    lastElementsInRange = elementsInRange;
}

void AdjustPanelPosition(Transform panel, Vector3 originalPos, Vector3 lookAtPos, float approachDistance)
{
    Vector3 direction = (lookAtPos - originalPos).normalized;
    Vector3 targetPos = originalPos + direction * approachDistance;
    panel.position = Vector3.Lerp(panel.position, targetPos, Time.deltaTime * rotationSpeed);
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

        Quaternion targetRotation = Quaternion.LookRotation(limitedDir);
        Quaternion correctedRotation = targetRotation * Quaternion.Euler(0f, 180f, 0f);
        target.rotation = Quaternion.Lerp(target.rotation, correctedRotation, Time.deltaTime * rotationSpeed);
    }

    Transform FindChildWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
            if (child.CompareTag(tag))
                return child;
        return null;
    }

void DisableAllDataPanels()
{
    GameObject[] allPanels = GameObject.FindGameObjectsWithTag("DataPanel");
    foreach (var panel in allPanels)
    {
        if (originalPanelPositions.TryGetValue(panel.transform, out Vector3 originalPos))
            panel.transform.position = originalPos;

        panel.SetActive(false);
    }

    originalPanelPositions.Clear();
}

}
