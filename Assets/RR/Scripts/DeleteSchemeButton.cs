using UnityEngine;
using UnityEngine.UI;

public class DeleteSchemeButton : MonoBehaviour
{
    public float radius = 3f;
    public float forwardOffset = 2f;

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
            Destroy(nearest);
        }
    }
}
