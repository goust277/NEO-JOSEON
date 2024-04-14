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
        Vector3 v;
        if (toTarget)
            v = actor.GetTarget().transform.position;
        else
            v = targetPos;
        if (invert)
            v *= -1;
        actor.SetTarget(v);
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
        Vector3 v;
        if (toTarget)
        {
            v = actor.GetTarget().transform.position;
            if (invert)
                v *= -1;
            actor.SetTarget(v);
        }
    }
}
