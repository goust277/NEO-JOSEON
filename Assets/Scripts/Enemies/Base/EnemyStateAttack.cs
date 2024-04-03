using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class EnemyStateAttack : EnemyState
{
    [Header("���� ����")]
    [SerializeField] protected float damage = 0f;
    [SerializeField] protected string property = "";
    [SerializeField] protected LayerMask layerMask; // ���� ��� ���̾�
    [Header("���� ������")]
    [SerializeField] protected float delayBefore = 0.3f; // ���� �� ������
    protected float delayBeforeCurr = 0;
    [SerializeField] protected float delayAfter = 0.3f; // ���� �� ������
    protected float delayAfterCurr = 0;
    [SerializeField] protected bool lookTargetOnBeforeDelay = false; // Ÿ�� ������Ʈ �ٶ󺸱� ����
    [SerializeField] protected float lookTime = 0.2f; // ���ʱ��� �ٶ� ������, delaybefore �̻��� ��� �׻�
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
