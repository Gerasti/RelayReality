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
[SerializeField]private static bool yChange = false;

[SerializeField]private static int yAngles;

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

        GameObject newElement = Instantiate(prefab, handShiftElement.position, handShiftElement.rotation);
         Debug.Log($"Created element: {newElement.name} with tag: {newElement.tag}");
        elements.Add(newElement);

        newElement.transform.SetParent(handShiftElement.transform);
        newElement.GetComponent<Rigidbody>().isKinematic = true;
 
       
    }

    public void CreateScheme(){
        Quaternion rotation = schemeH ? Quaternion.Euler(0, yAngles, 0) : Quaternion.Euler(90, yAngles, 0);
    
        GameObject newScheme = Instantiate(scheme, hand.position, rotation);
        schemes.Add(newScheme);

        newScheme.GetComponent<Rigidbody>().isKinematic = true;
    
         Debug.Log("Scheme Position: " + hand.position);
    }



public void DestroySchemes(){

foreach(GameObject scheme in schemes){

Destroy(scheme);

}
schemes.Clear();
}

}


