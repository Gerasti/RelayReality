using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ElementsAndSchemes : MonoBehaviour
{
   
    [SerializeField]private GameObject scheme;
    [SerializeField] private GameObject powerPrefab;
    [SerializeField] private GameObject resisterPrefab;
    public enum ElementType{
        Power,
        Resister
    }
   public Transform hand;
   public Transform handShiftElement;
   public Transform cameraTransform;

    

private bool schemeH;
private static bool yChange = false;

public static int yAngles;

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
//currentElementList
    [SerializeField] private List<GameObject> resisters = new();
    public List<GameObject> ResisterBuffer => resisters;

    [SerializeField] private List<GameObject> powers = new();
    public List<GameObject> PowersBuffer => powers;

//Schemes
    [SerializeField]private List<GameObject> schemes = new List<GameObject>();
    public List<GameObject> SchemesBuffer => schemes;
    public void SetSchemeOrientation(bool isHorizontal)
    {
        schemeH = isHorizontal;
    }

    public void CreatePowerElement()
    {
        CreateElement(powerPrefab, ElementType.Power);
    }

    public void CreateResisterElement()
    {
        CreateElement(resisterPrefab, ElementType.Resister);
    }

    public void CreateElement(GameObject prefab, ElementType type){
        
            List<GameObject> currentElementList = type switch
    {
        ElementType.Power => powers,
        ElementType.Resister => resisters,
        _ => throw new System.ArgumentException($"Unsupported element type: {type}")
    };


        GameObject newElement;

        if (currentElementList.Count > 0)
        {
            newElement = currentElementList[^1]; // ^1 — последний элемент
            currentElementList.RemoveAt(currentElementList.Count - 1);
            newElement.SetActive(true);
        }
        else
        {
            newElement = Instantiate(prefab);
            newElement.name = $"{type}";
        }

        newElement.transform.SetPositionAndRotation(handShiftElement.position, handShiftElement.rotation);
        newElement.GetComponent<Rigidbody>().isKinematic = true;
        newElement.transform.SetParent(handShiftElement);

    }

    public void CreateScheme(){

        Quaternion rotation = schemeH ? Quaternion.Euler(0, yAngles, 0) : Quaternion.Euler(90, yAngles, 0);
        Vector3 position = schemeH ? new Vector3(hand.position.x, hand.position.y-0.6f, hand.position.z) : hand.position;

        GameObject newScheme;

        if (schemes.Count > 0)
        {
            newScheme = schemes[^1];
            schemes.RemoveAt(schemes.Count - 1);
            newScheme.SetActive(true);
        }
        else
        {
            newScheme = Instantiate(scheme);
            newScheme.GetComponent<Rigidbody>().isKinematic = true;
        }

        newScheme.transform.SetPositionAndRotation(position, rotation);

    }


}


