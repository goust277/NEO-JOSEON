using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour, IDamageable
{
    [SerializeField] private int hp;
    [SerializeField] private float hitDelay;
    public bool isHit = false;
    private float delay = 0f;
    private bool isHitPosible = true;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
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

    }

    public void TakeDamage(Damage damage)
    {
        Debug.Log("Hit");
        if (isHitPosible)
        {
            StartCoroutine(Hit());
            isHitPosible = false;
            delay = 0f;
            animator.SetTrigger("Hit");
            //animator.ResetTrigger("Hit");
        }
    }

    IEnumerator Hit()
    {
        isHit = true;
        yield return new WaitForSecondsRealtime(0.4f);
        isHit = false;
    }

}
