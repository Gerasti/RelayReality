using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class DeleteSchemeButton : MonoBehaviour
{
    private float radius = 3f;
    private float forwardOffset = 2f;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(DeleteNearestScheme);
    }

    void DeleteNearestScheme()
    {
        Vector3 center = Camera.main.transform.position + Camera.main.transform.forward * forwardOffset;
        Collider[] hits = Physics.OverlapSphere(center, radius);

        GameObject nearest = null;
        float minDist = float.MaxValue;

        foreach (var col in hits)
        {
            if (col.CompareTag("Scheme"))
            {
                float dist = Vector3.Distance(col.transform.position, center);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = col.gameObject;
                }
            }
        }

        if (nearest != null)
        {
            EraseModeManager eraseManager = FindObjectOfType<EraseModeManager>();

            foreach(Transform child in nearest.transform)
            {
                foreach (Transform grandChild in child)
                {
                    if (grandChild.CompareTag("Element"))
                    {
                        eraseManager.EraseElement(grandChild.gameObject);
                    }
                }
            }

            ElementsAndSchemes schemesManager = FindObjectOfType<ElementsAndSchemes>();
            List<GameObject> schemes_buffer = schemesManager.Schemes_buffer;
            schemes_buffer.Add(nearest);

            nearest.SetActive(false);
        }
    }
}
