using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Boomer : Enemy
{
    EnemyStateMeleeArc meleeState;

    public bool detect = false;
    protected override void OnAwake()
    {
        SetDefaultState(0);
    }

    protected override void OnUpdate()
    {
        switch(StateCurrIdx)
        {
            case 0:
                if (detect)
                {
                    SetState(1);
                }
                break;
            case 1:
                if (meleeState.IsTargetInRange())
                {
                    SetState(2);
                }
                break;
            case 2:
                if (meleeState.IsAttackOver())
                {
                    
                    SetState(1);
                }
                break;
        }

        if (!detect)
        {
            SetState(0);
        }

    }
    
}
