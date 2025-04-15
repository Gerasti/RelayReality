using UnityEngine;
using Valve.VR.InteractionSystem; 
using System.Collections.Generic;
using System.Collections;
using JetBrains.Annotations;

public class EraseModeManager : MonoBehaviour
{
    public static bool eraseModeActive = false;
    private ElementsAndSchemes elementsManager;

    void Start()
    {
        elementsManager = FindObjectOfType<ElementsAndSchemes>();
    }

    public void SetEraseMode(bool isActive)
    {
        eraseModeActive = isActive;
    }

    public void EraseElement(GameObject elementToErase)
    {
        if (elementToErase != null)
        {
            elementToErase.transform.position = new Vector3(0, -100, 0);
            
            if (elementsManager != null)
            {
                List<GameObject> elements_buffer = elementsManager.Elements_buffer;
                elements_buffer.Add(elementToErase);
            }

            var rb = elementToErase.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            elementToErase.transform.SetParent(null);
        }
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
                    EraseElement(hit.collider.gameObject);
                }
            }
        }
    }
}