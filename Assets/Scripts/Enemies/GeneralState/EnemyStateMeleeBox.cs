using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyStateMeleeBox : EnemyStateAttack
{
    [Header("���� ����")]
    [SerializeField] private float meleeDetectionRange = 1f; // ���� ���� �ν� �Ÿ�
    [SerializeField] private float meleeRange = 5; // ���� ���� ���� �Ÿ�
    [SerializeField] private float meleeWidth = 1; // ���� ���� ���� �ʺ�
    [SerializeField][Min(0.1f)] private float attackHeight = 1f; // ���� ����
    [SerializeField] private float attackHeightOffset = 0f; // ���� ���� ����
    [SerializeField] private bool lookTargetOnBeforeDelay = true;

    [Header("Ÿ�� ���� ����")]
    [SerializeField] [Min(1)] private int attackWay = 1;
    [SerializeField] [Range(0f, 360f)] private float wayDiff = 90f;
    [SerializeField] [Range(0f, 360f)] private float diffOffset = 0f;
    private List<int> hits = new List<int>();

#if UNITY_EDITOR
    [Header("DEBUG")]
    [SerializeField] public bool VIEW_RANGE = false;

    private void OnDrawGizmos()
    {
        if (!VIEW_RANGE)
            return;
        Vector3 oriPos = transform.position;
        Vector3 hOffset = transform.up * attackHeightOffset;

        Handles.color = new Color(1f, 1f, 1f, 0.1f);
        Handles.DrawSolidDisc(transform.position, Vector3.up, meleeDetectionRange);

        for (int i = 0; i < attackWay; i++)
        {
            Quaternion rot = Quaternion.Euler(0, (wayDiff * (i - 0.5f * (attackWay - 1))) + diffOffset, 0);
            Vector3 width = rot * transform.right * meleeWidth * 0.5f;

            Vector3 p1 = oriPos + rot * width + hOffset;
            Vector3 p2 = oriPos - rot * width + hOffset;
            Vector3 p3 = p1 + rot * transform.forward * meleeRange;
            Vector3 p4 = p2 + rot * transform.forward * meleeRange;

            Vector3 attackH = transform.up * attackHeight;
            Vector3 p5 = p1 + attackH;
            Vector3 p6 = p2 + attackH;
            Vector3 p7 = p3 + attackH;
            Vector3 p8 = p4 + attackH;

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
    }
#endif

    public override void Attack()
    {
        actor.SetLookAtTarget(false);
        int selfId = gameObject.GetInstanceID();

        for (int i = 0; i < attackWay; i++)
        {
            Quaternion rot = Quaternion.Euler(0, (wayDiff * (i - 0.5f * (attackWay - 1))) + diffOffset, 0);
            Vector3 boxCenter = transform.position;
            boxCenter += rot * transform.forward * meleeRange * 0.5f;
            boxCenter.y += attackHeightOffset + attackHeight * 0.5f;

            Vector3 boxSize = new Vector3(meleeWidth, attackHeight, meleeRange);
            boxSize *= 0.5f;

            Vector3 trsRot = transform.rotation.eulerAngles;
            trsRot.y += (wayDiff * (i - 0.5f * (attackWay - 1))) + diffOffset;
            Quaternion boxRot = Quaternion.Euler(trsRot);

            Collider[] hit = Physics.OverlapBox(boxCenter, boxSize, boxRot, layerMask);

            foreach (Collider ele in hit)
            {
                IDamageable target = ele.GetComponent<IDamageable>();// ���� ������ ����ΰ�?
                if (target == null)
                    continue;

                int id = ele.GetInstanceID();
                if (id == selfId) // �ڽ��� ����
                    continue;

                if (hits.Contains(id)) // �̹� ģ ��� ����
                    continue;

                hits.Add(id);
                Damage d;
                d.amount = damage;
                d.property = property;
                target.TakeDamage(d);
            }
        }
        hits.Clear();
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

    // Ÿ���� ���� ���� �ȿ� �ִ���
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