using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyStateSlam : EnemyStateAttack
{
    private Rigidbody rb = null;

    [Header("이동 속성")]
    [SerializeField] private float jumpPower = 20f;
    [SerializeField] private float moveSpeed = 10f;
    private Vector3 targetPos = Vector3.zero;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEnter()
    {
        actor.SetAiActive(false);
        rb.isKinematic = false;
        targetPos = actor.GetTarget().transform.position;
    }

    public override void OnExit()
    {
        base.OnExit();
        rb.isKinematic = true;
        actor.SetAiActive(true);
    }

    public override void Attack()
    {
        isAttacking = true;
        rb.AddRelativeForce(transform.up * jumpPower, ForceMode.Impulse);
    }

    public override void OnAttacking()
    {
        Vector3 vec = targetPos - transform.position;
        vec.y = 0;
        if (vec.magnitude > 1)
            vec.Normalize();
        vec *= Time.deltaTime;
        vec *= moveSpeed;
        rb.MovePosition(vec + transform.position);

        // 나중에 판정 고치기
        if (rb.velocity.y <= 0 && Mathf.Abs(targetPos.y - transform.position.y) < 0.1f)
            isAttacking = false;
    }

    public override bool IsAttackOver()
    {
        return base.IsAttackOver() && !isAttacking;
    }

    public override bool CanAttackTarget()
    {
        return false;
    }
}
