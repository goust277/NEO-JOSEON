using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_jumper : Enemy, IDamageable
{
    [SerializeField] private int hp;
    [SerializeField] private float jumpPower;
    [SerializeField] private float jumpCoolDown;
    [SerializeField] private float Jdelay = 0f;

    public bool detect = false;
    public bool isJumpReady = true;
    public bool isGround = true;

    EnemyStateSlam meleeState;
    [SerializeField] private GameObject effect;
    protected override void OnAwake()
    {
        meleeState = GetComponent<EnemyStateSlam>();

        SetDefaultState(0);

        meleeState.SetCallbackHit(OnHit);
    }

    private void OnHit(Collider a)
    {
        
    }

    protected override void OnUpdate()
    {
        if(Jdelay < jumpCoolDown)
        {
            Jdelay += Time.deltaTime;
        }
        else
        {
            isJumpReady = true;
        }

        if (detect && isJumpReady)
        {
            SetState(1);
            Jdelay = 0f;
            isJumpReady = false;
        }
        else if (!detect && isGround)
        {
            SetState(0);
        }
    }

    private void isJumpReady_()
    {
        isJumpReady = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Plane"))
        {
            isGround = true;
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Plane"))
        {
            isGround = false;
        }
    }

}
