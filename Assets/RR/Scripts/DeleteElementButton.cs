using UnityEngine;
using UnityEngine.UI;

public class DeleteElementButton : MonoBehaviour
{
    public float maxDistance = 5f;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(DeleteElement);
    }

    void DeleteElement()
    {
        Camera cam = Camera.main;
        Vector3 origin = cam.transform.position;
        Vector3 direction = cam.transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance))
        {
            if (hit.collider.CompareTag("Element"))
            {
                Destroy(hit.collider.gameObject);
            }
            {
                Destroy(hit.collider.gameObject);
            }
        }
    }
}
