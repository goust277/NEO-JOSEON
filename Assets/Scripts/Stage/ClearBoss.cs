using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearBoss : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameData.Chapter = 2;
        }
    }
}
