using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

public class EnemyStateMeleeArc : EnemyStateAttack
{
    [Header("���� ����")]
    [SerializeField] private float meleeRange = 1f; // ���� ���� �Ÿ�
    [SerializeField] [Range(1, 180)] private float meleeAngle = 30; // ���� ���� ����
    [SerializeField] [Min(0.1f)] private float attackHeight = 1f; // ���� ����
    [SerializeField] private float attackHeightOffset = 0f; // ���� ���� ����

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

        Gizmos.DrawLine(vp, v1); // ���ݰ� 1
        Gizmos.DrawLine(vp, v2); // ���ݰ� 2

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
            IDamageable target = ele.GetComponent<IDamageable>();// ���� ������ ����ΰ�?
            if (target == null)
                continue;

            if (ele.GetInstanceID() == selfId) // �ڽ��� ����
                continue;

            if (Vector3.Distance(transform.position, ele.transform.position) > meleeRange)// ����� ��Ÿ� �̳��ΰ�?
                continue;

            Vector3 v1 = transform.forward;
            Vector3 v2 = (ele.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(v1, v2);
            if (dot < Mathf.Cos(Mathf.Deg2Rad * meleeAngle)) // ����� ���� ���� ���ΰ�?
                continue;

            Damage d;
            d.amount = damage;
            d.property = property;
            target.TakeDamage(d);
        }
    }

    // Ÿ���� ���� ���� �ȿ� �ִ���
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

    // ����� ���� ���� ���� �ִ��� Ȯ��
    public bool IsTargetInAngle()
    {
        Vector3 v1 = transform.forward;
        Vector3 v2 = (actor.GetTarget().transform.position - transform.position).normalized;
        float dot = Vector3.Dot(v1, v2);
        if (dot < Mathf.Cos(Mathf.Deg2Rad * meleeAngle)) // ����� ���� ���� ���ΰ�?
            return false;
        return true;
    }

    public override bool CanAttackTarget()
    {
        return IsTargetInAngle() && IsTargetInRange();
    }
}
