using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;

public class ClearTutorialRoom : MonoBehaviour
{
    [Header("¹®")]
    [SerializeField] private GameObject[] doors;

    private int MaxPoint;
    private int point = 0;

    [Header("Á¸")]
    [SerializeField] private GameObject[] zones;

    private void Start()
    {
        MaxPoint = zones.Length;
    }
    void Update()
    {
        
        if (point == MaxPoint)
        {
            for(int i = 0; i < doors.Length; i++)
            {
                Destroy(doors[i]);
            }
        }
        else
        {
            zones[point].SetActive(true);
        }
    }

    public void PointUp()
    {
        point++;
    }
}
