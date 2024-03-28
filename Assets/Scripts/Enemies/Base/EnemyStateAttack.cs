using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class EnemyStateAttack : EnemyState
{
    [Header("피해 정보")]
    [SerializeField] protected float damage = 0f;
    [SerializeField] protected string property = "";
    [SerializeField] protected LayerMask layerMask; // 공격 대상 레이어
    [Header("공격 딜레이")]
    [SerializeField] protected float delayBefore = 0.3f; // 공격 전 딜레이
    protected float delayBeforeCurr = 0;
    [SerializeField] protected float delayAfter = 0.3f; // 공격 후 딜레이
    protected float delayAfterCurr = 0;
    protected bool isAttacking = false;
    protected bool isAttackOver = false;

    public abstract void Attack();

    public virtual void OnAttacking() { }

    public override void OnUpdate()
    {
        float deltaTime = Time.deltaTime;
        if (delayBeforeCurr < delayBefore) // Before Attack Delay
        {
            delayBeforeCurr += deltaTime;
            if (delayBeforeCurr >= delayBefore) // Attack
                Attack();
        }
        else if (delayAfterCurr < delayAfter) // After Attack Delay
        {
            if (isAttacking)
                OnAttacking();
            else
                delayAfterCurr += deltaTime;
        }
        else
            isAttackOver = true;
    }

    public override void OnExit()
    {
        delayBeforeCurr = 0;
        delayAfterCurr = 0;
        isAttackOver = false;
    }

    public bool IsAttackOver()
    {
        return isAttackOver;
    }
}
