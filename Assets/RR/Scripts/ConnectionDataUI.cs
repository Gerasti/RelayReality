using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConnectionDataUI : MonoBehaviour
{
    public ConnectionData connectionData;

    public TextMeshProUGUI IText;
   // public Button IPlus, IMinus;

    public TextMeshProUGUI UText;
    public Button UPlus, UMinus;

    public TextMeshProUGUI RText;
    public Button RPlus, RMinus;

    void Start()
    {
       // IPlus.onClick.AddListener(() => connectionData.I += 0.5f);
       // IMinus.onClick.AddListener(() => connectionData.I -= 0.5f);

UPlus.onClick.AddListener(() => { connectionData.U += 0.5f; connectionData.dataChanged = true; });
UMinus.onClick.AddListener(() => { connectionData.U -= 0.5f; connectionData.dataChanged = true; });

RPlus.onClick.AddListener(() => { connectionData.R += 0.5f; connectionData.dataChanged = true; });
RMinus.onClick.AddListener(() => { connectionData.R -= 0.5f; connectionData.dataChanged = true; });

    }

    void Update()
    {
        IText.text = connectionData.I.ToString("F2");
        UText.text = connectionData.U.ToString("F2");
        RText.text = connectionData.R.ToString("F2");
    }
}
