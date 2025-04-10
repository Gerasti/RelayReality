using UnityEngine;
using UnityEngine.UI;

public class ConnectModeToggle : MonoBehaviour
{
    public Color activeColor = Color.green;
    public Color defaultColor = Color.white;

    private bool isActive = false;
    private Image image;
    private Button button;

    void Start()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(ToggleMode);
        image.color = defaultColor;
    }

    void ToggleMode()
    {
        isActive = !isActive;
        image.color = isActive ? activeColor : defaultColor;
        ConnectionManager cm = FindObjectOfType<ConnectionManager>();
        if (cm) cm.SetConnectMode(isActive);
    }
}
