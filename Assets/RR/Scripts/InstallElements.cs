using Unity.VisualScripting;
using UnityEngine;

public class InstallElements : MonoBehaviour
{
    bool IsHeld;
    private void OnTriggerEnter(Collider element)
    {
        if (element.CompareTag("Element") && transform.childCount == 0)
        {
            Debug.Log("MATCH Cell");
            HoldElements holdElements = element.GetComponent<HoldElements>();
            IsHeld = holdElements.IsHeld;

            if (IsHeld)
            {
                Debug.Log("Cell is held");
                return;
            }
            Transform elementTransform = element.transform;

            elementTransform.SetParent(transform, true);

            elementTransform.localPosition = Vector3.zero;
            elementTransform.rotation = Quaternion.Euler(0, transform.parent.eulerAngles.y, 0);

            Rigidbody rigidbody = elementTransform.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.isKinematic = true;
            }
            
        }
    }
}

