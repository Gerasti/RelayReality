using UnityEngine;

public class EraseModeManager : MonoBehaviour
{
    private bool eraseModeActive = false;

    public void SetEraseMode(bool isActive)
    {
        eraseModeActive = isActive;
    }

    void Update()
    {
        if (!eraseModeActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Element"))
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }
}
