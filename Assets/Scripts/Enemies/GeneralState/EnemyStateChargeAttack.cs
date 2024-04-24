using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyStateChargeAttack : EnemyStateAttack
{
    [Header("공격 방식")]
    [SerializeField] private float chargeSpeed = 20;
    [SerializeField] private float acceleration = 40;
    private float speedPrev = 0f; // 돌진 전 속도
    private float accelPrev = 0f;
    [SerializeField] private Transform dest = null; // 목표 지점, null이면 플레이어
    [Header("공격 범위")]
    [SerializeField] private float hitRadius = 1f;
    [SerializeField] private float hitHeight = 1f;

#if UNITY_EDITOR
    [Header("DEBUG")]
    [SerializeField] private bool VIEW_RANGE = false;
    private void OnDrawGizmos()
    {
        if (!VIEW_RANGE)
            return;
        Gizmos.color = Color.red;
        Vector3 height = new Vector3(0, Mathf.Max(0, hitHeight * 0.5f - hitRadius), 0);
        Gizmos.DrawWireSphere(transform.position + height, hitRadius);
        Gizmos.DrawWireSphere(transform.position - height, hitRadius);
        if (actor != null && !actor.IsChasing && delayAfterCurr <= 0)
        {
            Vector3 _dest;
            if (dest)
                _dest = dest.position;
            else
            {
                _dest = actor.GetTarget().transform.position;
                if (actor.HasPath)
                    _dest = actor.Dest;
            }
            Gizmos.DrawLine(transform.position, _dest);
        }
    }
#endif

    public override void AttackStart()
    {
        keepAttacking = true;
        if (dest)
            actor.SetTarget(dest.position);
        else
            actor.SetTarget(actor.GetTarget().transform.position);
    }

    public override void Attacking()
    {
        Vector3 height = new Vector3(0, Mathf.Max(0, hitHeight * 0.5f - hitRadius) + hitRadius, 0);
        Vector3 point1 = transform.position + height;
        Vector3 point2 = transform.position - height;
        Collider[] hit = Physics.OverlapCapsule(point1, point2, hitRadius, layerMask);
        int selfId = gameObject.GetInstanceID();

        foreach (Collider c in hit)
        {
            if (c.isTrigger)
                continue;
            IDamageable target = c.GetComponent<IDamageable>(); // 공격 가능한 대상인가?
            if (target == null)
                continue;

            int cID = c.GetInstanceID();
            if (cID == selfId) // 자신은 제외
                continue;
            
            if (CheckHitlist(cID))
            {
                Damage d;
                d.amount = damage;
                d.property = property;
                target.TakeDamage(d);
                onHitAttack?.Invoke(c);
            }
        }

        if (actor.IsArrived(0.5f))
            keepAttacking = false;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        actor.SetChase(false);
        speedPrev = actor.Speed;
        actor.Speed = chargeSpeed;
        accelPrev = actor.Acceleration;
        actor.Acceleration = acceleration;

#if UNITY_EDITOR
        VIEW_RANGE = true;
#endif
    }

    public override void OnExit()
    {
        base.OnExit();
        actor.Speed = speedPrev;
        actor.Acceleration = accelPrev;

#if UNITY_EDITOR
        VIEW_RANGE = false;
#endif
    }

    public override bool CanAttackTarget()
    {
        return true;
    }

    public void SetDest(Transform t)
    {
        dest = t;
    }
}
