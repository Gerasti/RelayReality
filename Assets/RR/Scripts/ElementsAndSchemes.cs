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
private static bool yChange = false;

private static int yAngles;

private void Update(){
    // Axis for creation schemes
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

    [SerializeField]public List<GameObject> elements = new List<GameObject>();
    public List<GameObject> Elements_buffer
    {   
    get { return elements; }
    private set { elements = value; }

    }

    [SerializeField]private List<GameObject> schemes = new List<GameObject>();
        public List<GameObject> Schemes_buffer 
    {   
    get { return schemes; }
    private set { schemes = value; }

    }
    public void HorizontalScheme(){
        schemeH = true;
    }

      public void VerticalScheme(){
        schemeH = false;
    }

    public void CreateElement(GameObject prefab){

        if(elements.Count > 0){
            GameObject newElement = elements[elements.Count - 1];
            elements.RemoveAt(elements.Count - 1);

            newElement.transform.SetParent(handShiftElement.transform);

            newElement.transform.position = handShiftElement.position;
            newElement.transform.rotation = handShiftElement.rotation;

            Debug.Log($"Return element: {newElement.name} with tag: {newElement.tag}");

        }else{
            GameObject newElement = Instantiate(prefab, handShiftElement.position, handShiftElement.rotation);

            Debug.Log($"Created element: {newElement.name} with tag: {newElement.tag}");

            newElement.transform.SetParent(handShiftElement.transform);
            newElement.GetComponent<Rigidbody>().isKinematic = true;
        }

    }

    public void CreateScheme(){

        Quaternion rotation = schemeH ? Quaternion.Euler(0, yAngles, 0) : Quaternion.Euler(90, yAngles, 0);

        if(schemes.Count > 0 ){
            GameObject newScheme = schemes[schemes.Count - 1];
            schemes.RemoveAt(schemes.Count - 1);

            newScheme.SetActive(true);
            newScheme.transform.position = hand.position;
            newScheme.transform.rotation = rotation;

            
            Debug.Log($"Return scheme: {newScheme.name} with tag: {newScheme.tag}");
        }else{
        
    
        GameObject newScheme = Instantiate(scheme, hand.position, rotation);

        newScheme.GetComponent<Rigidbody>().isKinematic = true;
    
         Debug.Log($"Create scheme: {newScheme.name} with tag: {newScheme.tag}");
        }
    }


}


