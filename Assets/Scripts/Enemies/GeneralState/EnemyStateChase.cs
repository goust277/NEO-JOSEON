using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateChase : EnemyState
{
    public override void OnEnter()
    {
        actor.SetChase(true);
        TrySetAnimBool("Idle", false);
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
    }
}
