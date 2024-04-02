using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomer_Detect : MonoBehaviour
{
    public Enemy_Boomer enemy;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            enemy.detect = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            enemy.detect = false;
        }
    }
}
