using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConnectionDataUI : MonoBehaviour
{
    public ConnectionData connectionData;

    public TextMeshProUGUI IText;

    public TextMeshProUGUI UText;
    public Button UPlus, UMinus;

    public TextMeshProUGUI RText;
    public Button RPlus, RMinus;

    void Start()
    {

        if (UPlus != null && UMinus != null)
        {
            UPlus.onClick.AddListener(() => { connectionData.U += 0.5f; connectionData.dataChanged = true; });
            UMinus.onClick.AddListener(() => { connectionData.U -= 0.5f; connectionData.dataChanged = true; });
        }
        if (RPlus != null && RMinus != null)
        {
            RPlus.onClick.AddListener(() => { connectionData.R += 0.5f; connectionData.dataChanged = true; Debug.Log("PLUS R"); });
            RMinus.onClick.AddListener(() => { connectionData.R -= 0.5f; connectionData.dataChanged = true; Debug.Log("MINUS R"); });
        }
    }

    void Update()
    {
        IText.text = connectionData.I.ToString("F2");
        UText.text = connectionData.U.ToString("F2");
        RText.text = connectionData.R.ToString("F2");
    }
}
