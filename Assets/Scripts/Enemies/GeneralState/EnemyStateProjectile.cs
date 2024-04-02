using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateProjectile : EnemyStateAttack
{
    [Header("발사체 큐")]
    [SerializeField] private ProjectileQueue projQueue = null;

    [Header("발사체 속성")]
    [SerializeField] private float velocity = 1f;
    [SerializeField] private float range = 5f;
    [SerializeField] private bool fixYPos = true;

    public override void Attack()
    {
        TrySetAnimBool("Attack", true);
        Projectile p = projQueue.GetProjectile();
        if (p == null) return;

        p.SetOwnerID(gameObject.GetInstanceID());

        Damage d;
        d.amount = damage;
        d.property = property;
        p.SetDamage(d);

        p.SetRange(range);

        p.SetLayerMask(ref layerMask);

        Vector3 actorTransform = actor.transform.position;
        Vector3 speedVector = actor.GetTarget().transform.position - actorTransform;
        if (fixYPos)
            speedVector.y = actorTransform.y;
        p.SetSpeedVector(speedVector.normalized * velocity);

        p.Fire(actorTransform);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        actor.SetChase(false);
        TrySetAnimBool("Idle", true);
    }

    public override void OnExit()
    {
        base.OnExit();

        TrySetAnimBool("Attack", false);
    }

    public override bool CanAttackTarget()
    {
        return true;
    }
}
