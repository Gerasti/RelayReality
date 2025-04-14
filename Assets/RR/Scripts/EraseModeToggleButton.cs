using UnityEngine;
using UnityEngine.UI;

public class EraseModeToggleButton : MonoBehaviour
{
    public Color activeColor = Color.green;
    public Color defaultColor = Color.white;

    private bool isActive = false;
    private Image image;
    private Button button;
    private EraseModeManager eraseManager;

    void Start()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(ToggleEraseMode);
        image.color = defaultColor;
        eraseManager = FindObjectOfType<EraseModeManager>();
    }

    void ToggleEraseMode()
    {
        isActive = !isActive;
        Debug.Log("isActive: "+ isActive);
        image.color = isActive ? activeColor : defaultColor;
        if (eraseManager != null)
            eraseManager.SetEraseMode(isActive);
    }
}
