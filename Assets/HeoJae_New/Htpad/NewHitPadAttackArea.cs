using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHitPadAttackArea : MonoBehaviour
{
    public float fDamage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerDamage>().TakeDamage();
            return;
        }
    }
}
