using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1_AttackArea : MonoBehaviour
{
    public float fDamage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerDamage>().TakeDamage(fDamage);
            return;
        }
        if (other.CompareTag("HitPad"))
        {
            NewHitPad newHitPad = other.gameObject.GetComponent<NewHitPad>();
            newHitPad.GetHitGauge();
            return;
        }

    }
}
