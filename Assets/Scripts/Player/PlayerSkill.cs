using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    public float skillRange;
    public float pushForce;

    public LayerMask enemyLayer, HitLayer;

    private float delay;

    private Animator animator;
    [SerializeField] private GameObject effect;

    public bool isSkillTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (delay < 3f)
        {
            delay += Time.deltaTime;
        }

    }
    public void TriggerSkill()
    {
        isSkillTime = true;
        Invoke("SkillOff", 1f);
        Collider[] colliders = Physics.OverlapSphere(transform.position, skillRange, enemyLayer);
        Collider[] hidCols = Physics.OverlapSphere(transform.position, skillRange, HitLayer);
        animator.SetTrigger("Skill");

        Invoke("SkillEffect", 0.3f);

        foreach (Collider collider in colliders)
        {
            Rigidbody enemyRigidbody = collider.GetComponent<Rigidbody>();
            if (enemyRigidbody != null)
            {
                //enemyRigidbody.isKinematic = false;
                Vector3 pushDirection = (collider.transform.position - transform.position).normalized;

                enemyRigidbody.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }
            delay = 0;
            if (delay > 1f)
            {
                //enemyRigidbody.isKinematic = true;
            }

        }

        foreach (Collider collider in hidCols)
        {
            HitPad hitPad = collider.GetComponent<HitPad>();
            hitPad.HitDown();

        }
        
    }
    private void SkillEffect()
    {
        GameObject spawnEffect = Instantiate(effect, transform.position, Quaternion.identity);
        Destroy(spawnEffect, 0.7f);
    }

    private void SkillOff()
    {
        isSkillTime = false;
    }
}
