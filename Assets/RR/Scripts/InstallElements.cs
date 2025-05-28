using Unity.VisualScripting;
using UnityEngine;

public class InstallElements : MonoBehaviour
{
    bool IsHeld;
    Vector3 installPosition = new Vector3(0, 3.5f, 0);

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

            elementTransform.localPosition = installPosition;


            if (transform.eulerAngles.z == 180f)
            {

                elementTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                Debug.Log("Родитель повернут на 180, устанавливаем локальный Z = 0");
            }
            else
            {

                elementTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }


            Rigidbody rigidbody = elementTransform.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.isKinematic = true;
            }
        }
    }
}
