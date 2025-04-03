using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HoldElements : MonoBehaviour
{
    //public SteamVR_Action_Boolean grabAction; 
    //public SteamVR_Input_Sources handType;

    public bool IsInstalled = false;
    public bool IsHeld = true; //Test
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
        //Test
        
    if (transform.parent == null && IsHeld || IsInstalled)
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
            IsInstalled = false;
}

    public void HoldObject()
    {
            GetComponent<Rigidbody>().isKinematic = true;
            IsHeld = true;
}
}