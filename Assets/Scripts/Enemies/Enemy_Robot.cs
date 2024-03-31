using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Robot : Enemy
{
    [Header("추적")]
    [SerializeField] private float chaseTimeMax = 7f; // 추적 시간


    [Header("상태 전환")]
    [SerializeField][Range(0f, 1f)] private float chargeChance = 0.5f;
    [SerializeField][Range(0f, 1f)] private float swingChance = 0.5f;
    [SerializeField][Range(0f, 1f)] private float hitDownChance = 0.5f;

    protected override void OnAwake()
    {
        SetDefaultState(0);
    }

    protected override void OnUpdate()
    {
        switch (StateCurrIdx)
        {
            case 0: // 추적
                /*if (StateDuration > chaseTimeMax)
                    SetState(1);*/
                break;
        }
    }
}