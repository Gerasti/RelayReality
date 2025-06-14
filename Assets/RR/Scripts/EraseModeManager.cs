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
    if (elementToErase != null && elementsManager != null)
    {
        
        var elementTypeName = elementToErase.name;
        ElementsAndSchemes.ElementType type = System.Enum.Parse<ElementsAndSchemes.ElementType>(elementTypeName);

        switch (type)
        {
            case ElementsAndSchemes.ElementType.Power:
                elementsManager.PowersBuffer.Add(elementToErase);
                break;
            case ElementsAndSchemes.ElementType.Resister:
                elementsManager.ResisterBuffer.Add(elementToErase);
                break;
            default:
                Debug.LogError($"Unknown element type: {type}");
                return;
        }

        Transform parent = elementToErase.transform.parent;

        if (parent != null && parent.GetComponent<InstallElements>() != null)
        {
            elementToErase.transform.SetParent(null);
        }

        if (parent == elementsManager.handShiftElement)
        {
            elementToErase.SetActive(false);
            return;
        }

        var rb = elementToErase.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        elementToErase.transform.position = new Vector3(0, -100, 0);
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
                    EraseElement(hit.collider.gameObject);
                }
            }
        }
    }
}