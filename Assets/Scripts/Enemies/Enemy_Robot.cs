using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Robot : Enemy
{
    [Header("����")]
    [SerializeField] private float chaseTimeMax = 7f; // ���� �ð�

    [Header("���� ��ȯ")]
    [SerializeField][Range(0f, 1f)] private float chargeChance = 0.5f;
    [SerializeField][Range(0f, 1f)] private float swingChance = 0.5f;

    EnemyStateMeleeArc melee1State;
    EnemyStateChargeAttack chargeState;
    EnemyStateAttack projectileState;
    EnemyStateMeleeArc swingState;
    protected override void OnAwake()
    {
        melee1State = (EnemyStateMeleeArc)StateList[1];
        chargeState = (EnemyStateChargeAttack)StateList[2];
        projectileState = (EnemyStateAttack)StateList[3];
        swingState = (EnemyStateMeleeArc)StateList[4];

        SetDefaultState(0);
    }

    protected override void OnUpdate()
    {
        switch (StateCurrIdx)
        {
            case 0: // ����
                if (melee1State.CanAttackTarget())
                {
                    if (Random.value < swingChance && swingState.CanAttackTarget())
                        SetState(4);
                    else
                        SetState(1);
                }
                if (StateDuration > chaseTimeMax)
                {
                    if (Random.value < chargeChance)
                        SetState(2);
                    else
                        SetState(3);
                }
                break;
            case 1: // ���� 1
                if (melee1State.IsAttackOver())
                    SetState(0);
                break;
            case 2: // ����
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
        }
    }
}