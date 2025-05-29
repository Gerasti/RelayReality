using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HoldElements : MonoBehaviour
{
    public bool IsHeld = true;
    bool eraseModeActive;
    GameObject handShiftElement;

    void Start()
    {
        handShiftElement = GameObject.FindWithTag("HandShift");
    }
    private void Update()
    {
        eraseModeActive = EraseModeManager.eraseModeActive;

        if (eraseModeActive == true && transform.parent?.gameObject == handShiftElement)
        {

            EraseModeManager eraseManager = FindObjectOfType<EraseModeManager>();
            if (eraseManager != null)
            {
                eraseManager.EraseElement(gameObject);
            }
            return;
        }

        if (transform.parent == null && IsHeld)
        {
            FreeObject();
            return;
        }

        if (transform.parent != null && !IsHeld)
        {
            HoldObject();
            return;
        }

        

    }

    public void FreeObject()
    {
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        IsHeld = false;
    }

    public void HoldObject()
    {
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        IsHeld = true;
    }
}
