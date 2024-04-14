using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyStateProjectile : EnemyStateAttack
{
    [Header("발사체 큐")]
    [SerializeField] private ProjectileQueue projQueue = null;

    [Header("발사체 속성")]
    [SerializeField] private float velocity = 1f;
    [SerializeField] private float range = 5f;
    [SerializeField] private bool piercing = false;
    [SerializeField] private bool fixYPos = true;

    [Header("발사체 수")]
    [SerializeField] [Min(1)] private int shotWays = 1;
    [SerializeField][Range(0f, 360f)] private float wayDiff = 90f;
    [SerializeField][Range(0f, 360f)] private float diffOffset = 0f;

    public int ShotWays
    {
        get { return shotWays; }
        set { shotWays = value; }
    }

    public float WayDiff
    {
        get { return wayDiff; }
        set { wayDiff = value; }
    }

    public float DiffOffset
    {
        get { return diffOffset; }
        set { diffOffset = value; }
    }

#if UNITY_EDITOR
    [Header("DEBUG")]
    [SerializeField] public bool VIEW_RANGE = false;

    private void OnDrawGizmos()
    {
        if (!VIEW_RANGE)
            return;
        Vector3 oriPos = transform.position;

        Gizmos.color = Color.red;

        for (int i = 0; i < shotWays; i++)
        {
            Quaternion rot = Quaternion.Euler(0, (wayDiff * (i - 0.5f * (shotWays - 1))) + diffOffset, 0);
            Vector3 direction =  rot * transform.forward;

            Vector3 destPos = oriPos + direction * range;
            Vector3 oneSecPos = oriPos + direction * velocity;

            Gizmos.DrawLine(oriPos, destPos);
            Gizmos.DrawWireSphere(oneSecPos, 1f);
        }
    }
#endif

    public override void AttackStart()
    {
        FireProjectile();
    }

    private void FireProjectile()
    {
        Damage d;
        d.amount = damage;
        d.property = property;
        Vector3 actorTransform = actor.transform.position;
        for (int i = 0; i < shotWays; i++)
        {
            Projectile p = projQueue.GetProjectile();
            if (p == null) return;

            Quaternion rot = Quaternion.Euler(0, (wayDiff * (i - 0.5f * (shotWays - 1))) + diffOffset, 0);
            p.SetOwnerID(gameObject.GetInstanceID());
            p.SetDamage(d);
            p.SetRange(range);
            p.SetLayerMask(ref layerMask);
            p.SetPiercing(piercing);

            Vector3 speedVector = (actor.GetTarget().transform.position - actorTransform);
            if (fixYPos)
                speedVector.y = actorTransform.y;
            p.SetSpeedVector(rot * speedVector.normalized * velocity);
            p.Fire(actorTransform);
        }
    }

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

    public override bool CanAttackTarget()
    {
        return true;
    }
}
