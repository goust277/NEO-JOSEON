using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Robot : Enemy
{
    [Header("추적")]
    [SerializeField] private float chaseTimeMax = 7f; // 추적 시간


    [Header("상태 전환")]
    [SerializeField][Range(0f, 1f)] private float chargeChance = 0.5f;
    [SerializeField][Range(0f, 1f)] private float swingChance = 0.5f;
    [SerializeField][Range(0f, 1f)] private float hitDownChance = 0.5f;

    EnemyStateMeleeArc melee1State;
    EnemyStateChargeAttack chargeState;

    EnemyStateAttack projectileState;
    EnemyStateMeleeArc swingState;
    EnemyStateMeleeBox melee2State;
    protected override void OnAwake()
    {
        melee1State = (EnemyStateMeleeArc)StateList[1];
        chargeState = (EnemyStateChargeAttack)StateList[2];

        projectileState = (EnemyStateAttack)StateList[3];
        swingState = (EnemyStateMeleeArc)StateList[4];
        melee2State = (EnemyStateMeleeBox)StateList[5];

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
                    if (Random.value < swingChance && swingState.CanAttackTarget())
                        SetState(4);
                    else
                    {
                        if (Random.value < hitDownChance && melee2State.CanAttackTarget())
                            SetState(5);
                        else
                            SetState(1);
                    }
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

           case 3:
                if (projectileState.IsAttackOver())
                    SetState(0);
                break;
            case 4:
                if (swingState.IsAttackOver())
                    SetState(0);
                break;
            case 5:
                if (melee2State.IsAttackOver())
                    SetState(0);
                break;
        }
    }
}