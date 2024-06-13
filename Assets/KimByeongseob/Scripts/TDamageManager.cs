using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDamageManager : MonoBehaviour
{
    public float fDamage = 1f;

    void Start()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerDamage>().TakeDamage(fDamage);
            return;
        }
    }
}
