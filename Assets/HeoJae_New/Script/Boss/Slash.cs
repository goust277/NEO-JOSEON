using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    [Header("ÆÄÆ¼Å¬")]
    public ParticleSystem particleFire;

    public float fDamage;
    public float moveSpeed;
    private bool bIsUse = false;


    private void Awake()
    {
        Destroy(gameObject, 2.7f);
    }


    void Update()
    {
        transform.Translate(transform.forward * Time.deltaTime * moveSpeed, Space.World);
    }


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
