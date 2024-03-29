using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyStateChargeAttack : EnemyStateAttack
{
    private List<int> hits = new List<int>(); // 타격 대상들의 ID 저장
    [Header("공격 방식")]
    [SerializeField] private float chargeSpeed = 20;
    [SerializeField] private float acceleration = 40;
    private float speedPrev = 0f; // 돌진 전 속도
    private float accelPrev = 0f;
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
            Vector3 dest = actor.GetTarget().transform.position;
            if (actor.HasPath)
                dest = actor.Dest;
            Gizmos.DrawLine(transform.position, dest);
        }
    }
#endif

    public override void Attack()
    {
        TrySetAnimBool("Idle", false);
        TrySetAnimFloat("Speed", 3);
        isAttacking = true;
        actor.SetTarget(actor.GetTarget().transform.position);
        actor.SetLookAtTarget(false);
    }

    public override void OnAttacking()
    {
        Vector3 height = new Vector3(0, Mathf.Max(0, hitHeight * 0.5f - hitRadius) + hitRadius, 0);
        Vector3 point1 = transform.position + height;
        Vector3 point2 = transform.position - height;
        Collider[] hit = Physics.OverlapCapsule(point1, point2, hitRadius, layerMask);
        int selfId = gameObject.GetInstanceID();

        foreach (Collider c in hit)
        {
            IDamageable target = c.GetComponent<IDamageable>(); // 공격 가능한 대상인가?
            if (target == null)
                continue;

            int cID = c.GetInstanceID();
            if (cID == selfId) // 자신은 제외
                continue;

            if (hits.Contains(cID)) // 이미 한 번 공격한 대상인가?
                continue;
            hits.Add(cID);

            Damage d;
            d.amount = damage;
            d.property = property;
            target.TakeDamage(d);
        }

        if (actor.IsArrived(0.5f))
            isAttacking = false;
    }

    public override void OnEnter()
    {
        TrySetAnimBool("Idle", true);
        actor.SetChase(false);
        speedPrev = actor.Speed;
        actor.Speed = chargeSpeed;
        accelPrev = actor.Acceleration;
        actor.Acceleration = acceleration;
    }

    public override void OnExit()
    {
        base.OnExit();
        TrySetAnimFloat("Speed", 1);
        actor.Speed = speedPrev;
        actor.Acceleration = accelPrev;
        hits.Clear();
        actor.SetLookAtTarget(true);
    }
}
