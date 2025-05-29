using UnityEngine;
using System;
using System.Collections.Generic;


public class ConnectionData : MonoBehaviour
{
    public float I = 0.0f;

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
                OnAnyDataChanged?.Invoke();
            }
        }
    }

    public float U
    {
        get => u;
        set
        {
            if (Mathf.Abs(u - value) > 0.01f)
            {
                u = value;
                dataChanged = true;
                OnAnyDataChanged?.Invoke();
            }
        }
    }

    [SerializeField] private float r = 1.0f;
    [SerializeField] private float u = 0.0f;

}
