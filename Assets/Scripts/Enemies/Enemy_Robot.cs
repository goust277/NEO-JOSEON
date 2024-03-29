using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Robot : Enemy
{
    [Header("추적")]
    [SerializeField] private float chaseTimeMax = 7f; // 추적 시간

    EnemyStateMeleeArc melee1State;
    EnemyStateChargeAttack chargeState;
<<<<<<< HEAD

=======
    EnemyStateAttack projectileState;
    EnemyStateMeleeArc swingState;
>>>>>>> upstream/main
    protected override void OnAwake()
    {
        melee1State = (EnemyStateMeleeArc)StateList[1];
        chargeState = (EnemyStateChargeAttack)StateList[2];
        SetDefaultState(0);
    }

    protected override void OnUpdate()
    {
        switch (StateCurrIdx)
        {
            case 0: // 추적
                if (melee1State.IsTargetInRange() &&
                    melee1State.IsTargetInAngle())
                {
                    SetState(1);
                }
                if (StateDuration > chaseTimeMax)
                    SetState(2);
                break;
            case 1: // 근접 1
                if (melee1State.IsAttackOver())
                    SetState(0);
                break;
            case 2: // 돌진
                if (chargeState.IsAttackOver())
                    SetState(0);
                break;
        }
    }
}