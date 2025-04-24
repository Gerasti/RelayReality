using UnityEngine;

public enum PowerType 
{
    Positive,
    Negative
}

public class PowerPoint : MonoBehaviour
{
    public PowerType type;
    public float voltage = 5f;

    void Start()
    {
        var connectionData = GetComponent<ConnectionData>();

        connectionData.U = type == PowerType.Positive ? voltage : 0f;
    }
}