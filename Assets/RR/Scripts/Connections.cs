using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connections : MonoBehaviour
{
    private LineRenderer Line;
    private Transform Hand; // Камера игрока
    private Transform firstEmpty; // Первый Empty объект
    private bool isFirstCollision = true; // Флаг для отслеживания первой и второй коллизии

    void Start()
    {

    GameObject handObject = GameObject.FindWithTag("HandShift");
        if (handObject != null)
        {
            Hand = handObject.transform;
        }
        else
        {
            Debug.LogError("Объект с тегом 'HandShift' не найден!");
        }

        Line.startColor = Color.red;
        Line.endColor = Color.red;

        Line.startWidth = 0.3f;
        Line.endWidth = 0.3f;
        Line.positionCount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ConnectionPoint")) 
        {
            if (isFirstCollision)
            {
                firstEmpty = other.transform; 
                Line.positionCount = 2;
                Line.SetPosition(0, firstEmpty.position); 
                Line.SetPosition(1, Hand.position); 
                isFirstCollision = false;
            }
            else
            {
                Transform secondEmpty = other.transform; 
                Line.SetPosition(1, secondEmpty.position); 
                isFirstCollision = true; 
            }
        }
    }

    void Update()
    {
        if (!isFirstCollision && firstEmpty != null)
        {
            Line.SetPosition(1, Hand.position); 
        }
    }
}