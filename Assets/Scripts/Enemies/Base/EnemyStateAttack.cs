using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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
    [SerializeField] protected float attackDuration = 0.1f; // 공격 지속 시간
    protected float attackDurationCurr = 0;
    [SerializeField] protected float delayAfter = 0.3f; // 공격 후 딜레이
    protected float delayAfterCurr = 0;


    private List<int> hits = new List<int>(); // 타격 대상들의 ID 저장

    [SerializeField] protected bool lookTargetOnBeforeDelay = false; // 타겟 오브젝트 바라보기 여부
    [SerializeField] protected float lookTime = 0.2f; // 몇초까지 바라볼 것인지, delaybefore 이상인 경우 항상
    protected bool keepAttacking = false; // true일 때 공격 지속 시간을 무시
    protected bool isAttackOver = false;
    protected bool isAttacking = false;

    public delegate void Callback();
    public delegate void Callback2(Collider c);
    protected Callback onStartAttack = null;
    protected Callback onEndAttack = null;
    protected Callback2 onHitAttack = null;

    public float DelayBefore
    {
        get { return delayBefore; }
        set { delayBefore = value; }
    }

    public float AttackDuration
    {
        get { return attackDuration; }
        set { attackDuration = value; }
    }

    public float DelayAfter
    {
        get { return delayAfter; }
        set { delayAfter = value; }
    }
    public float LookTime
    {
        get { return lookTime; }
        set { lookTime = value; }
    }

    public enum CallbackType
    {
        ON_START_ATTACK,
        ON_END_ATTACK
    }

    public void MultTiming(float m)
    {
        delayBefore *= m;
        delayAfter *= m;
        attackDuration *= m;
        lookTime *= m;
    }

    public void SetCallbackTiming(Callback cb, CallbackType t)
    {
        switch (t)
        {
            case CallbackType.ON_START_ATTACK:
                onStartAttack = cb;
                break;
            case CallbackType.ON_END_ATTACK:
                onEndAttack = cb;
                break;
        }
    }

    public void SetCallbackHit(Callback2 cb)
    {
        onHitAttack = cb;
    }

    public virtual void AttackStart() { }

    public virtual void Attacking() { }

    public override void OnUpdate()
    {
        float deltaTime = Time.deltaTime;
        if (delayBeforeCurr < delayBefore) // Before Attack Delay
        {
            delayBeforeCurr += deltaTime;
            if (delayBeforeCurr >= delayBefore) // Attack
            {
                onStartAttack?.Invoke();
                isAttacking = true;
                AttackStart();
            }
        }
        else if ((attackDurationCurr < attackDuration) || keepAttacking)
        {
            Attacking();
            attackDurationCurr += deltaTime;
        }
        else if (delayAfterCurr < delayAfter) // After Attack Delay
        {
            isAttacking = false;
            delayAfterCurr += deltaTime;
            if (delayAfterCurr >= delayAfter)
            {
                isAttackOver = true;
                onEndAttack?.Invoke();
            }
        }

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
        attackDurationCurr = 0;
        isAttackOver = false;
        hits.Clear();
    }

    public virtual bool IsAttackOver()
    {
        return isAttackOver;
    }

    public virtual bool IsAttacking()
    {
        return keepAttacking;
    }

    public abstract bool CanAttackTarget();

    protected bool CheckHitlist(int id)
    {
        if (hits.Contains(id)) // 이미 한 번 공격한 대상인가?
            return false;
        hits.Add(id);
        return true;
    }

    protected void ClearHitlist()
    {
        hits.Clear();
    }
}
