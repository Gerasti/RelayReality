using UnityEngine;
using UnityEngine.UI;

public class RadiusUIActivator : MonoBehaviour
{
    public SphereCollider detectionCollider;
    public Button toggleButton;
    public Transform source;              // Камера или игрок
    public float rotationSpeed = 5f;
    public float maxVerticalAngle = 30f;

    private void Start()
    {
        detectionCollider.enabled = false;
        toggleButton.onClick.AddListener(ToggleDetection);
    }

    void ToggleDetection()
    {
        bool newState = !detectionCollider.enabled;
        detectionCollider.enabled = newState;

        if (!newState)
            DisableAllDataPanels();
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
            Transform panel = panelObj.transform;
            SmoothLookAt(panel, source);
        }
    }

    void SmoothLookAt(Transform target, Transform lookAt)
    {
        Vector3 direction = lookAt.position - target.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        // Убираем вертикальную составляющую
        Vector3 flatDir = new Vector3(direction.x, 0f, direction.z).normalized;
        float verticalAngle = Vector3.Angle(direction, flatDir);
        float sign = Mathf.Sign(direction.y);
        float limitedAngle = Mathf.Min(verticalAngle, maxVerticalAngle);
        Vector3 limitedDir = Quaternion.AngleAxis(limitedAngle * sign, Vector3.Cross(flatDir, Vector3.up)) * flatDir;

        if (limitedDir.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(limitedDir);
        target.rotation = Quaternion.Lerp(target.rotation, targetRotation, Time.deltaTime * rotationSpeed);
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
}
