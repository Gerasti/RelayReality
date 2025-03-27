using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ElementsAndSchemes : MonoBehaviour
{
   
   public GameObject scheme;

   public Transform hand;
   public Transform camera;

private bool schemeV;
private float yAngles;

private void Update(){

if((camera.rotation.y >= 0.38f && camera.rotation.y <= 0.92f ) || (camera.rotation.y >= -0.92f && camera.rotation.y <= -0.38f ))
{
    yAngles = 90;
}else{
    yAngles = 0;
}

}

    private List<GameObject> elements = new List<GameObject>();
    private List<GameObject> schemes = new List<GameObject>();

    public void HorizontalScheme(){
        schemeV = false;
    }

      public void VerticalScheme(){
        schemeV = true;
    }

    public void CreateElement(GameObject prefab){
        if(hand.childCount == 0){
       // prefab.transform.position = new Vector3(0.8f, 1.75f, 3.5f);
        GameObject newElement = Instantiate(prefab, hand.position += new Vector3(0.8f, 0, -1), hand.rotation);
        elements.Add(newElement);

        newElement.transform.SetParent(hand.transform);
        newElement.GetComponent<Rigidbody>().isKinematic = true;
 
       Debug.Log("Elements Position: " + hand.position);
        }
    }

    public void CreateScheme(){
        Quaternion rotation = schemeV ? Quaternion.Euler(90, yAngles, 0) : Quaternion.Euler(0, yAngles, 0);
    
        GameObject newScheme = Instantiate(scheme, hand.position, rotation);
        schemes.Add(newScheme);

  newScheme.transform.SetParent(hand.transform);
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


