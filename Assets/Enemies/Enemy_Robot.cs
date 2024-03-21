using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Robot : Enemy
{
    [Header("추적")]
    [SerializeField] private float chaseTimeMax = 7f; // 추적 시간

    EnemyStateMeleeArc Melee1State;

    protected override void OnAwake()
    {
        Melee1State = (EnemyStateMeleeArc)StateList[1];
        SetDefaultState(0);
    }

    protected override void OnUpdate()
    {
        switch (StateCurrIdx)
        {
            case 0: // 추적
                if (Melee1State.IsTargetInRange() &&
                    Melee1State.IsTargetInAngle())
                {
                    SetState(1);
                }
                if (StateDuration > chaseTimeMax)
                    Debug.Log("CHARGE ATTACK!"); // Transit to ChargeAttack / RangedAttack
                break;
            case 1: // 근접 1
                if (Melee1State.IsAttackOver())
                    SetState(0);
                break;
        }
    }
}