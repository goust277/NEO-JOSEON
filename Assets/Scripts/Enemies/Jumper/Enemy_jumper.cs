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
    public bool isJump = false;

    protected override void OnAwake()
    {
        SetDefaultState(0);
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

        //switch (StateCurrIdx)
        //{
        //    case 0:
        //        if (detect && isJumpReady)
        //        {
        //            SetState(1);
        //            Jdelay = 0f;
        //            isJumpReady = false;
        //        }

        //        break;
        //    case 1:
        //        if (!detect && !isJump)
        //        {
        //            SetState(0);
        //        }
        //        break;
        //    case 2:
        //        if (!isJumpReady && !isJump)
        //        {
        //            SetState(0);
        //        }
        //        break;

        //}
        if (detect && isJumpReady)
        {
            SetState(1);
            Jdelay = 0f;
            isJumpReady = false;
        }
        else if (!detect && !isJump)
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
            isJump = false;
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Plane"))
        {
            isJump = true;
        }
    }

}
