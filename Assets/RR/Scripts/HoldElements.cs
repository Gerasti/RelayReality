using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HoldElements : MonoBehaviour
{
    public SteamVR_Action_Boolean grabAction; 
    public SteamVR_Input_Sources handType;

    private bool hasDetached = false; //Test

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

        //Test
    if(CompareTag("Element")){
        
    if (transform.parent == null && !hasDetached)
    {
        FreeObject();
    }
    if(transform.parent != null && hasDetached){
        HoldObject();
    }
    }
}
    

    public void FreeObject()
    {
   
            GetComponent<Rigidbody>().isKinematic = false;
            hasDetached = true;
}

    public void HoldObject()
    {
            GetComponent<Rigidbody>().isKinematic = true;
            hasDetached = false;
}
}