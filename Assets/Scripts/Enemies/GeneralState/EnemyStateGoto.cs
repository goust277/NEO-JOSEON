using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateGoto : EnemyState
{
    [Header("이동")]
    [SerializeField] private float moveSpeed = 20;
    [SerializeField] private float acceleration = 40;
    [SerializeField] private Vector3 targetPos = Vector3.zero;
    [SerializeField] private bool toTarget = false;
    [SerializeField] private bool backStep = false;
    [SerializeField] private float backStepDist = 2f;
    private float speedPrev = 0f; // 돌진 전 속도
    private float accelPrev = 0f;

    public override void OnEnter()
    {
        actor.SetChase(false);
        Vector3 v = Vector3.zero;
        Vector3 targetPos = actor.GetTarget().transform.position;
        Vector3 actorPos = actor.transform.position;
        if (toTarget)
        {
            if (backStep)
                v = actorPos + (actorPos - targetPos).normalized * backStepDist;
            else
                v = targetPos;

        }
        else
            v = targetPos;
        actor.SetTarget(v);
        speedPrev = actor.Speed;
        actor.Speed = moveSpeed;
        accelPrev = actor.Acceleration;
        actor.Acceleration = acceleration;
    }

    public override void OnExit()
    {
        actor.Speed = speedPrev;
        actor.Acceleration = accelPrev;
    }

    public override void OnUpdate()
    {
        /*
        Vector3 v = Vector3.zero;
        Vector3 targetPos = actor.GetTarget().transform.position;
        Vector3 actorPos = actor.transform.position;
        if (toTarget)
        {
            if (invert)
                v = 2 * actorPos - targetPos;
            else
                v = targetPos;
            actor.SetTarget(v);
        }*/
    }
}
