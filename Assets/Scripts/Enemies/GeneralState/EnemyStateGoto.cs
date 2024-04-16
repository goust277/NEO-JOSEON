using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateGoto : EnemyState
{
    [SerializeField] private Vector3 targetPos = Vector3.zero;
    [SerializeField] private bool toTarget = false;
    [SerializeField] private bool invert = false;

    public override void OnEnter()
    {
        actor.SetChase(false);
        Vector3 v = Vector3.zero;
        Vector3 targetPos = actor.GetTarget().transform.position;
        Vector3 actorPos = actor.transform.position;
        if (toTarget)
        {
            if (invert)
                v = 2 * actorPos - targetPos;
            else
                v = targetPos;
        }
        else
            v = targetPos;
        actor.SetTarget(v);
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
        Vector3 v = Vector3.zero;
        Vector3 targetPos = actor.GetTarget().transform.position;
        Vector3 actorPos = actor.transform.position;
        if (toTarget)
        {
            if (invert)
                v = 2 * actorPos - targetPos;
            else
                v = targetPos;
            actor.SetTarget(v);
        }
    }
}
