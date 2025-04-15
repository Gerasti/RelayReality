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
        
    if (eraseModeActive == true && transform.parent?.gameObject == handShiftElement){
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
    }
    if(transform.parent != null && !IsHeld){
        HoldObject();
    }
    
}

    public void FreeObject()
    {
   
            GetComponent<Rigidbody>().isKinematic = false;
            IsHeld = false;
}

    public void HoldObject()
    {
            GetComponent<Rigidbody>().isKinematic = true;
            IsHeld = true;
}
}