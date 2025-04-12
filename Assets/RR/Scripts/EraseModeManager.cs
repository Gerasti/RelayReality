using UnityEngine;
using Valve.VR.InteractionSystem; 

public class EraseModeManager : MonoBehaviour
{
    public static bool eraseModeActive = false;

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
                    // Interactable interactable = hit.collider.GetComponent<Interactable>();
                    // if (interactable != null)
                    // {
                    //     Hand hand = interactable.attachedToHand;
                    //     if (hand != null)
                    //     {
                    //         hand.DetachObject(hit.collider.gameObject);
                    //     }
                    // }
                    hit.transform.SetParent(null);

                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }
}