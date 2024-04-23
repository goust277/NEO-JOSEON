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
        if (this.CompareTag("Key"))
        {
            if (keyPad == null)
                return;
            keyPad.OpenCancle();
        }
        else 
        {
            if (keyPad == null)
                return;
            keyPad.notOpen--;
        }
            
    }
    
    public void HitDown()
    {
        if (hitGauge > 0)
        {
            Debug.Log("Pad");
            hitGauge--;
            if (keyPad != null)
            {
                if (keyPad.notOpen > 0 && hitGauge == 0)
                {
                    keyPad.notOpen--;

                }
            }

            if (this.CompareTag("Key"))
            {
                if (keyPad == null)
                    return;
                keyPad.OpenCancle();
            }
        }

    }

    private void Puzzle()
    {
        if (hit)
        {
            if (this.CompareTag("Key"))
            {
                if (keyPad == null)
                    return;
                keyPad.AddOpen();
            }
            else
            {
                if (keyPad == null)
                    return;
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
