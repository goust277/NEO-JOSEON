using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyStateMeleeBox : EnemyStateAttack
{
    [Header("공격 범위")]
    [SerializeField] private float meleeDetectionRange = 1f; // 근접 공격 인식 거리
    [SerializeField] private float meleeRange = 5; // 근접 공격 판정 거리
    [SerializeField] private float meleeWidth = 1; // 근접 공격 판정 너비
    [SerializeField][Min(0.1f)] private float attackHeight = 1f; // 공격 높이
    [SerializeField] private float attackHeightOffset = 0f; // 공격 높이 보정
    [SerializeField] private bool lookTargetOnBeforeDelay = true;

#if UNITY_EDITOR
    [Header("DEBUG")]
    [SerializeField] public bool VIEW_RANGE = false;

    private void OnDrawGizmos()
    {
        if (!VIEW_RANGE)
            return;
        Vector3 width = transform.right * meleeWidth * 0.5f;
        Vector3 oriPos = transform.position;
        Vector3 hOffset = transform.up * attackHeightOffset;

        Vector3 p1 = oriPos + width + hOffset;
        Vector3 p2 = oriPos - width + hOffset;
        Vector3 p3 = p1 + transform.forward * meleeRange;
        Vector3 p4 = p2 + transform.forward * meleeRange;

        Vector3 attackH = transform.up * attackHeight;
        Vector3 p5 = p1 + attackH;
        Vector3 p6 = p2 + attackH;
        Vector3 p7 = p3 + attackH;
        Vector3 p8 = p4 + attackH;

        Handles.color = new Color(1f, 1f, 1f, 0.1f);
        Handles.DrawSolidDisc(transform.position, Vector3.up, meleeDetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p1, p3);
        Gizmos.DrawLine(p2, p4);

        Gizmos.DrawLine(p1, p5);
        Gizmos.DrawLine(p2, p6);
        Gizmos.DrawLine(p3, p7);
        Gizmos.DrawLine(p4, p8);

        Gizmos.DrawLine(p5, p6);
        Gizmos.DrawLine(p7, p8);
        Gizmos.DrawLine(p5, p7);
        Gizmos.DrawLine(p6, p8);
    }
#endif

    public override void Attack()
    {
        actor.SetLookAtTarget(false);
        Vector3 boxCenter = transform.position;
        boxCenter += transform.forward * meleeRange * 0.5f;
        boxCenter.y += attackHeightOffset + attackHeight * 0.5f;
        Vector3 boxSize = new Vector3(meleeWidth, attackHeight, meleeRange);
        boxSize *= 0.5f;

        Collider[] hit = Physics.OverlapBox(boxCenter, boxSize, transform.rotation, layerMask);

        int selfId = gameObject.GetInstanceID();

        foreach (Collider ele in hit)
        {
            IDamageable target = ele.GetComponent<IDamageable>();// 공격 가능한 대상인가?
            if (target == null)
                continue;

            if (ele.GetInstanceID() == selfId) // 자신은 제외
                continue;

            Damage d;
            d.amount = damage;
            d.property = property;
            target.TakeDamage(d);
        }
    }

    public override void OnEnter()
    {
        actor.SetChase(false);
        actor.SetLookAtTarget(lookTargetOnBeforeDelay);
        TrySetAnimBool("Attack", true);
    }
    public override void OnExit()
    {
        base.OnExit();
        actor.SetLookAtTarget(true);
        TrySetAnimBool("Attack", false);
    }

    // 타겟이 근접 범위 안에 있는지
    public bool CanAttackTarget()
    {
        GameObject target = actor.GetTarget();
        if (target == null)
            return false;

        float distToTarget = (target.transform.position - transform.position).magnitude;
        if (distToTarget > meleeDetectionRange)
            return false;
        return true;
    }
}
