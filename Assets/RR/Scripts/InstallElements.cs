using UnityEngine;

public class InstallElements : MonoBehaviour
{
    private void OnTriggerEnter(Collider element)
    {
        if (element.CompareTag("Element"))
        {
            HoldElements holdElements = element.GetComponent<HoldElements>();
            if (holdElements != null && holdElements.IsHeld)
            {
                return;
            }

            holdElements.IsInstalled = true;

            Transform elementTransform = element.transform;

            elementTransform.position = transform.position;
            elementTransform.rotation = Quaternion.Euler(0, transform.parent.parent.rotation.y, 0);
            Debug.Log("Element Position: " + elementTransform.rotation);

            Rigidbody rigidbody = elementTransform.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.isKinematic = true;
            }
        }
    }
}

