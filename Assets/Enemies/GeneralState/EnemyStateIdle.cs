using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateIdle : EnemyState
{
    public override void OnEnter()
    {
        actor.SetChase(false);
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
    }
}
