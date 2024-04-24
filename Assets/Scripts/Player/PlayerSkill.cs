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
        if (delay < 0.2f)
        {
            delay += Time.deltaTime;
        }
    }
    public void TriggerSkill()
    {
        StartCoroutine("Skill_");
    }
    public void StopSkill()
    {
        StopCoroutine("Skill_");
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

    IEnumerator Skill_()
    {
        isSkillTime = true;
        Invoke("SkillOff", 1.2f);

        animator.SetTrigger("Skill");
        Invoke("Skill", 0.8f);
        Invoke("SkillEffect", 0.3f);

        yield return null;
    }
    private void Skill()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, skillRange, enemyLayer);
        Collider[] hidCols = Physics.OverlapSphere(transform.position, skillRange, HitLayer);


        foreach (Collider collider in colliders)
        {
            Rigidbody enemyRigidbody = collider.GetComponent<Rigidbody>();
            if (enemyRigidbody != null)
            {
                enemyRigidbody.velocity = Vector3.zero;

                enemyRigidbody.isKinematic = false;
                Vector3 pushDirection = (collider.transform.position - transform.position).normalized;

                enemyRigidbody.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }
        }
        StartCoroutine(ResetEnemyVelocity(colliders, 0.1f));
        foreach (Collider collider in hidCols)
        {
            HitPad hitPad = collider.GetComponent<HitPad>();
            if (collider.isTrigger == true)
                return;
            hitPad.HitDown();
        }
    }
    IEnumerator ResetEnemyVelocity(Collider[] colliders, float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (Collider collider in colliders)
        {
            Rigidbody enemyRigidbody = collider.GetComponent<Rigidbody>();
            if (enemyRigidbody != null)
            {
                enemyRigidbody.isKinematic = true;
                enemyRigidbody.velocity = Vector3.zero; // 적의 속도를 0으로 설정
            }
        }
    }
}
