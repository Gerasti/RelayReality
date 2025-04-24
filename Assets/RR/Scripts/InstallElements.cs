using Unity.VisualScripting;
using UnityEngine;

public class InstallElements : MonoBehaviour
{
    bool IsHeld;
    private void OnTriggerEnter(Collider element)
    {
        if (element.CompareTag("Element") && transform.childCount == 0)
        {
            HoldElements holdElements = element.GetComponent<HoldElements>();
            IsHeld = holdElements.IsHeld;

            if (IsHeld) return;
            Transform elementTransform = element.transform;

Vector3 originalWorldScale = elementTransform.lossyScale;

            elementTransform.SetParent(transform, true);

        Vector3 parentWorldScale = transform.lossyScale;
        Vector3 newLocalScale = new Vector3(
            originalWorldScale.x / parentWorldScale.x,
            originalWorldScale.y / parentWorldScale.y,
            originalWorldScale.z / parentWorldScale.z
        );
        elementTransform.localScale = newLocalScale;

            elementTransform.localPosition = new Vector3(0, 3.5f, 0);
            elementTransform.rotation = Quaternion.Euler(transform.parent.eulerAngles.x, transform.parent.eulerAngles.y, 0);

            Rigidbody rigidbody = elementTransform.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.isKinematic = true;
            }
            
        }
    }
}

