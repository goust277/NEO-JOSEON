using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

public class EnemyStateMeleeArc : EnemyStateAttack
{
    [Header("공격 범위")]
    [SerializeField] private float meleeRange = 1f; // 근접 공격 거리
    [SerializeField] [Range(1, 180)] private float meleeAngle = 30; // 근접 공격 각도
    [SerializeField] [Min(0.1f)] private float attackHeight = 1f; // 공격 높이
    [SerializeField] private float attackHeightOffset = 0f; // 공격 높이 보정

#if UNITY_EDITOR
    [Header("DEBUG")]
    [SerializeField] public bool VIEW_RANGE = false;

    private void OnDrawGizmos()
    {
        if (!VIEW_RANGE)
            return;
        Handles.color = new Color(1f, 1f, 1f, 0.1f);
        Handles.DrawSolidDisc(transform.position, Vector3.up, meleeRange);

        if (delayBefore <= delayBeforeCurr && delayAfterCurr <= 0.12f)
        {
            Handles.color = new Color(1f, 0, 0, 0.3f);
            Gizmos.color = Color.magenta;
        }
        else
        {
            Handles.color = new Color(1f, 0, 0, 0.1f);
            Gizmos.color = Color.blue;
        }
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, meleeAngle, meleeRange);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -meleeAngle, meleeRange);

        float rad = meleeAngle * Mathf.Deg2Rad;
        float rotationRad = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
        Vector3 direction = transform.position + transform.forward * meleeRange;
        Vector3 v1 = transform.position;
        v1.x += Mathf.Sin(rotationRad + rad) * meleeRange;
        v1.z += Mathf.Cos(rotationRad + rad) * meleeRange;
        Vector3 v2 = transform.position;
        v2.x += Mathf.Sin(rotationRad - rad) * meleeRange;
        v2.z += Mathf.Cos(rotationRad - rad) * meleeRange;

        Vector3 vp = transform.position;

        Vector3 vho = new Vector3(0f, attackHeightOffset, 0f);
        v1 += vho;
        v2 += vho;
        vp += vho;
        direction += vho;

        Gizmos.DrawLine(vp, v1); // 공격각 1
        Gizmos.DrawLine(vp, v2); // 공격각 2

        Vector3 vh = new Vector3(0f, attackHeight, 0f);
        Vector3 vhp = vp + vh;
        Gizmos.DrawLine(vhp, v1 + vh);
        Gizmos.DrawLine(vhp, v2 + vh);

        Gizmos.DrawLine(vp, vp + vh);
        Gizmos.DrawLine(v1, v1 + vh);
        Gizmos.DrawLine(v2, v2 + vh);
    }
#endif

    public override void OnEnter()
    {
        base.OnEnter();
        actor.SetChase(false);

#if UNITY_EDITOR
        VIEW_RANGE = true;
#endif
    }

    public override void OnExit()
    {
        base.OnExit();

#if UNITY_EDITOR
        VIEW_RANGE = false;
#endif
    }

    public override void Attack()
    {
        Vector3 pos1 = transform.position;
        pos1.y += attackHeightOffset + attackHeight * 0.5f;
        Vector3 size = new Vector3(meleeRange, attackHeight * 0.5f, meleeRange);

        Collider[] hit = Physics.OverlapBox(pos1, size, transform.rotation,layerMask);
        if (hit.Length < 1)
            return;

        int selfId = gameObject.GetInstanceID();

        foreach (Collider ele in hit)
        {
            IDamageable target = ele.GetComponent<IDamageable>();// 공격 가능한 대상인가?
            if (target == null)
                continue;

            if (ele.GetInstanceID() == selfId) // 자신은 제외
                continue;

            if (Vector3.Distance(transform.position, ele.transform.position) > meleeRange)// 대상이 사거리 이내인가?
                continue;

            Vector3 v1 = transform.forward;
            Vector3 v2 = (ele.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(v1, v2);
            if (dot < Mathf.Cos(Mathf.Deg2Rad * meleeAngle)) // 대상이 공격 각도 안인가?
                continue;

            Damage d;
            d.amount = damage;
            d.property = property;
            target.TakeDamage(d);
        }
    }

    // 타겟이 근접 범위 안에 있는지
    public bool IsTargetInRange()
    {
        GameObject target = actor.GetTarget();
        if (target == null)
            return false;

        float distToTarget = (target.transform.position - transform.position).magnitude;
        if (distToTarget > meleeRange)
            return false;
        return true;
    }

    // 대상이 공격 각도 내에 있는지 확인
    public bool IsTargetInAngle()
    {
        Vector3 v1 = transform.forward;
        Vector3 v2 = (actor.GetTarget().transform.position - transform.position).normalized;
        float dot = Vector3.Dot(v1, v2);
        if (dot < Mathf.Cos(Mathf.Deg2Rad * meleeAngle)) // 대상이 공격 각도 안인가?
            return false;
        return true;
    }

    public override bool CanAttackTarget()
    {
        return IsTargetInAngle() && IsTargetInRange();
    }
}
