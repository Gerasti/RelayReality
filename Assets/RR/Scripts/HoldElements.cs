using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HoldElements : MonoBehaviour
{
    //public SteamVR_Action_Boolean grabAction; 
    //public SteamVR_Input_Sources handType;
    public bool IsHeld = true; //Test
    bool eraseModeActive;
    GameObject handShiftElement;

    void Start()
    {
        handShiftElement = GameObject.FindWithTag("HandShift");
    }
    private void Update()
    {
        // if (grabAction.GetStateUp(handType)) 
        // {
        //     FreeObject();
        // }

        // if (grabAction.GetStateDown(handType)) 
        // {
        //     HoldObject();
        // }
        //
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




        // Автоматическое освобождение объекта, если его отпустили
        if (transform.parent == null && IsHeld)
        {
            FreeObject();
            return;
        }

        // Автоматическое взятие объекта, если он прикреплён к чему-то
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
