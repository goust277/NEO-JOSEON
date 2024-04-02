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
    [SerializeField] protected bool lookTargetOnBeforeDelay = false; // 타겟 오브젝트 바라보기 여부
    [SerializeField] protected float lookTime = 0.2f; // 몇초까지 바라볼 것인지, delaybefore 이상인 경우 항상
    protected bool isAttacking = false;
    protected bool isAttackOver = false;

    public float DelayBefore
    {
        get { return delayBefore; }
    }

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
        if (lookTargetOnBeforeDelay &&
            lookTime < delayBeforeCurr)
            actor.SetLookAtTarget(false);
    }

    public override void OnEnter()
    {
        actor.SetLookAtTarget(lookTargetOnBeforeDelay);
    }

    public override void OnExit()
    {
        actor.SetLookAtTarget(true);
        delayBeforeCurr = 0;
        delayAfterCurr = 0;
        isAttackOver = false;
    }

    public virtual bool IsAttackOver()
    {
        return isAttackOver;
    }

    public abstract bool CanAttackTarget();
}
