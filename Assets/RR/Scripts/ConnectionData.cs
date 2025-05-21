using UnityEngine;
using System;

public class ConnectionData : MonoBehaviour
{
    public float I = 0.0f;
    public float U = 0.0f;

    public bool dataChanged = false;

    public static event Action OnAnyDataChanged;

    public float R
    {
        get => r;
        set
        {
            if (Mathf.Abs(r - value) > 0.01f)
            {
                r = value;
                dataChanged = true;
                OnAnyDataChanged?.Invoke(); // сигнал всем PowerPoint
            }
        }
    }

    [SerializeField] private float r = 1.0f;
}
