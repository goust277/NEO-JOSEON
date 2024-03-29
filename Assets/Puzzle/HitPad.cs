using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class HitPad : MonoBehaviour, IDamageable
{
    [SerializeField]private bool hit = false;
    [SerializeField]private Material[] matarials;

    [SerializeField] private float resetTime;

    [SerializeField] private int hitGauge = 0;

    private Renderer rend;
    public KeyPad keyPad;
    private float delay = 0f;
    public void TakeDamage(Damage damage)
    {
        Debug.Log("hit");
        if (hitGauge != 3)
        {
            if (damage.property.Contains("fire"))
            {
                hitGauge++;

                if (!hit)
                {
                    hit = true;
                    Puzzle();
                }
            }
        }

    }

    public void ClearPad()
    {
        hit = false;
        hitGauge = 0;
        if (this.CompareTag("key"))
        {
            keyPad.OpenCancle();
        }
        else 
        {
            keyPad.notOpen--;
        }
            
    }
    
    public void HitDown()
    {
        if (hitGauge > 0)
        {
            hitGauge--;
            if(keyPad.notOpen > 0 && hitGauge == 0)
            {
                keyPad.notOpen--;
            }
            if (this.CompareTag("key"))
            {
                keyPad.OpenCancle();
            }
        }

    }

    private void Puzzle()
    {
        if (hit)
        {
            if (this.CompareTag("key"))
            {
                Debug.Log("key");
                keyPad.AddOpen();
            }
            else
            {
                keyPad.CloseDoor();
                keyPad.notOpen++;
            }
        }
    }

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    
    void Update()
    {
        if (hitGauge == 0)
        {
            hit = false;
        }
        if (hitGauge == 3)
        {
            if (delay < resetTime)
            {
                delay += Time.deltaTime;
            }
            else
            {
                ClearPad();
                delay = 0;
            }

        }
        rend.material = matarials[hitGauge];
    }
}
