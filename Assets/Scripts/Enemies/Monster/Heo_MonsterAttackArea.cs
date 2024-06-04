using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heo_MonsterAttackArea : MonoBehaviour
{
    public float fDamage;
    public bool bIsCapter1Monster;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerDamage>().TakeDamage(fDamage);
            return;
        }
        else if(other.CompareTag("HitPad") && bIsCapter1Monster)
        {
            NewHitPad newHitPad = other.gameObject.GetComponent<NewHitPad>();
            newHitPad.GetHitGauge();
            return;
        }

    }
}
