using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LinePulse : MonoBehaviour
{
    public Color pulseColor = Color.blue;
    public float pulseSpeed = 1f;

    private Color baseColor;
    private LineRenderer lineRenderer;
    private float t;
    private bool isActive = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        baseColor = lineRenderer.material.color;
    }

    void Update()
    {
        if (!isActive) return;

        t += Time.deltaTime * pulseSpeed;
        float lerp = (Mathf.Sin(t) + 1f) / 2f;

Color pulse = new Color(pulseColor.r, pulseColor.g, pulseColor.b, 1f);
Color newColor = Color.Lerp(baseColor, pulse, lerp);

        lineRenderer.startColor = newColor;
        lineRenderer.endColor = newColor;
        lineRenderer.material.color = newColor;
    }

    // <--- Вот этот метод обязателен
    public void SetActive(bool state)
    {
        isActive = state;

        if (!state)
        {
            lineRenderer.startColor = baseColor;
            lineRenderer.endColor = baseColor;
            lineRenderer.material.color = baseColor;
            t = 0f;
        }
    }
}
