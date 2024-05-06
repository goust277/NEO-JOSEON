using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] private int hp;
    [SerializeField] private float hitDelay;
    public bool isHit = false;
    private float delay = 0f;
    private bool isHitPosible = true;
    private Animator animator;
    private PlayerMove hit;

    void Start()
    {
        animator = GetComponent<Animator>();
        hit = GetComponent<PlayerMove>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hitDelay > delay)
        {
            delay += Time.deltaTime;
        }
        else
        {
            isHitPosible = true;
        }
        if(hp <= 0)
        {
            Destroy(gameObject, 0.5f);
        }
    }

    public void TakeDamage()
    {
        if (isHitPosible)
        {
            StartCoroutine(Hit());
            isHitPosible = false;
            delay = 0f;
            animator.SetTrigger("Hit");
            hit.TakeDamage();
            hp--;
        }
    }

    IEnumerator Hit()
    {
        isHit = true;
        yield return new WaitForSecondsRealtime(0.4f);
        isHit = false;
    }

}
