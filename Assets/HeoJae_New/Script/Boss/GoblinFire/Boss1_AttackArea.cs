using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1_AttackArea : MonoBehaviour
{
    private bool bIsUse = false;
    public float fDamage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !bIsUse)
        {
            bIsUse = true;
            other.gameObject.GetComponent<PlayerDamage>().TakeDamage();
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
