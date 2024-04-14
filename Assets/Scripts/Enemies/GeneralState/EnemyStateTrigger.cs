using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateTrigger : EnemyStateAttack
{
    public override bool CanAttackTarget()
    {
        return true;
    }
}
