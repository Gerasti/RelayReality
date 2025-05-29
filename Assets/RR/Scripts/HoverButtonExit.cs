using UnityEngine;
using Valve.VR.InteractionSystem;

public class HoverButtonExit : MonoBehaviour
{
    private HoverButton hoverButton;

    private void Awake()
    {
        hoverButton = GetComponent<HoverButton>();
        hoverButton.onButtonDown.AddListener(OnButtonDown);
    }

    private void OnDestroy()
    {
        hoverButton.onButtonDown.RemoveListener(OnButtonDown);
    }

    private void OnButtonDown(Hand hand)
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
