using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Boomer : Enemy, IDamageable
{
    [SerializeField] private int hp;

    EnemyStateMeleeArc meleeState;

    public bool detect = false;

    public GameObject bomb;
    protected override void OnAwake()
    {
        meleeState = (EnemyStateMeleeArc)StateList[2];
        SetDefaultState(0);
        
        bomb.SetActive(false);
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
                    Invoke("BombActiveTrue", 0.5f);
                    SetState(2);
                    Invoke("BombActiveFalse", 1.5f);
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

    private void BombActiveTrue()
    {
        bomb.SetActive(true);
    }
    private void BombActiveFalse()
    {
        bomb.SetActive(false);
    }


}
