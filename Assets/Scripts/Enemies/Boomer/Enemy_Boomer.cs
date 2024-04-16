using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Boomer : Enemy
{
    EnemyStateMeleeArc meleeState;

    public bool detect = false;
    [SerializeField] private GameObject effect;
    protected override void OnAwake()
    {
        meleeState = GetComponent<EnemyStateMeleeArc>();

        SetDefaultState(0);
        meleeState.SetCallbackHit(OnHit);

        GameObject target = GameObject.FindGameObjectWithTag("Player");
        SetTarget(target);
    }
    private void OnHit(Collider a)
    {
        GameObject spawnEffect = Instantiate(effect, transform.position, Quaternion.identity);

        Destroy(spawnEffect, 1.2f);
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
