using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerSkill : MonoBehaviour
{
    private Coroutine coroutine;
    public float skillRange;
    public float pushForce;

    public LayerMask enemyLayer, HitLayer;

    private float delay;

    private Animator animator;
    [SerializeField] private ParticleSystem effect;
    GameObject spawnEffect;

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
        coroutine = StartCoroutine(Skill_());
    }
    public void StopSkill()
    {
        if(coroutine != null) 
        {
            StopCoroutine(coroutine);
            coroutine = null;
            isSkillTime = false;
            effect.Stop();
        }
    }

    IEnumerator Skill_()
    {
        isSkillTime = true;

        animator.SetTrigger("Skill");
        effect.Play();

        yield return new WaitForSeconds(0.4f);
        Skill();

        yield return new WaitForSeconds(0.4f);
        effect.Stop();
        

        isSkillTime = false;

        yield return null;
    }
    private void Skill()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, skillRange, enemyLayer);
        Collider[] hidCols = Physics.OverlapSphere(transform.position, skillRange, HitLayer);


        foreach (Collider collider in colliders)
        {
            NewEnemy enemy = collider.GetComponent<NewEnemy>();
            if (enemy != null)
            {
                enemy.GetComponent<NewEnemy>().TakeDamage(2);
            }
        }
        foreach (Collider collider in hidCols)
        {
            NewHitPad hitPad = collider.GetComponent<NewHitPad>();

            hitPad.CoolingHitGauge();
        }
    }
}
