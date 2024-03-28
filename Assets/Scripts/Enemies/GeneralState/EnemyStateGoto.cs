using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateGoto : EnemyState
{
    [SerializeField] private Vector3 targetPos = Vector3.zero;

    public override void OnEnter()
    {
        actor.SetChase(false);
        actor.SetTarget(targetPos);
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
    }
}
