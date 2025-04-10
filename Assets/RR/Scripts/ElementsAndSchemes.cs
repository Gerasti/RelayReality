using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ElementsAndSchemes : MonoBehaviour
{
   
   public GameObject scheme;

   public Transform hand;
   public Transform handShiftElement;
   public Transform cameraTransform;

    

private bool schemeH;
public static bool yChange = false;

public static int yAngles;
// private Vector3 elementPos;
// private Quaternion elementRot;

private void Update(){
    
    if (((cameraTransform.rotation.y >= 0.38f && cameraTransform.rotation.y <= 0.92f) || 
         (cameraTransform.rotation.y >= -0.92f && cameraTransform.rotation.y <= -0.38f)) && !yChange)
    {
        yAngles = 90;
        yChange = true;
    }
    else if(((cameraTransform.rotation.y < 0.38f && cameraTransform.rotation.y > -0.38f) || 
         (cameraTransform.rotation.y > 0.92f) || (cameraTransform.rotation.y < -0.92f)) && yChange)
    {
        yAngles = 0;
        yChange = false;
    }
        // Debug.Log("ROTATION: "+ cameraTransform.rotation.y);
        // Debug.Log("CHANGE: "+yChange);
}

    private List<GameObject> elements = new List<GameObject>();
    private List<GameObject> schemes = new List<GameObject>();

    public void HorizontalScheme(){
        schemeH = true;
    }

      public void VerticalScheme(){
        schemeH = false;
    }

    public void CreateElement(GameObject prefab){
        //if(hand.childCount == 0){
            // Debug.Log("1) Elements Position: " + elementPos);
            //elementPos += new Vector3(0.8f, 0, -1);
            // Debug.Log("2) Elements Position: " + elementPos);
        GameObject newElement = Instantiate(prefab, handShiftElement.position, handShiftElement.rotation);
        elements.Add(newElement);

        newElement.transform.SetParent(handShiftElement.transform);
        newElement.GetComponent<Rigidbody>().isKinematic = true;
 
    //  elementPos = hand.position;
    //    Debug.Log("Renew Elements Position: " + elementPos);
       
    }

    public void CreateScheme(){
        Quaternion rotation = schemeH ? Quaternion.Euler(0, yAngles, 0) : Quaternion.Euler(90, yAngles, 0);
    
        GameObject newScheme = Instantiate(scheme, hand.position, rotation);
        schemes.Add(newScheme);

  //newScheme.transform.SetParent(hand.transform);
        newScheme.GetComponent<Rigidbody>().isKinematic = true;
    
         Debug.Log("Scheme Position: " + hand.position);
    }

public void DestroyElements(){

foreach(GameObject element in elements){

Destroy(element);

}
elements.Clear();
}

public void DestroySchemes(){

foreach(GameObject scheme in schemes){

Destroy(scheme);

}
schemes.Clear();
}

}


