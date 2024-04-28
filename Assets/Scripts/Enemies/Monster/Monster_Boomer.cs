using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Monster_Boomer : MonoBehaviour ,IDamageable
{
    private StageManagerAssist stagemanager;

    [Header("몬스터 정보")]
    public int currentHp;

    [Header("몬스터 상태 / 관련 오브젝트")]
    public bool isChase = true;
    public bool isAttack;
    public bool doDie;

    [Header("공격 범위")]
    public GameObject attackArea;
    public float targetRadius;
    public float targetRange;
    public float AttackChargeTime;
    public bool bChargeStart;

    [Header("이펙트")]
    public ParticleSystem attackParticle;

    [Header("애니메이션 / 콜라이더")]
    public Animator anim;
    public new BoxCollider collider;
    private Transform player; // 목표로 하는 플레이어 위치
    private Rigidbody rb;
    private NavMeshAgent nav;

    void Start()
    {
        stagemanager = FindObjectOfType<StageManagerAssist>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!doDie)
        {
            nav.SetDestination(player.position);

            if (!isChase)
            {
                FixPosition(transform.position);
                nav.isStopped = true; // NavMeshAgent 멈춤
                nav.speed = 0;
                nav.angularSpeed = 0;
                FreezeMonster();
            }
            else
            {
                nav.isStopped = false; // NavMeshAgent 멈춤
                nav.speed = 3f;
                nav.angularSpeed = 600;
            }

            // 타겟팅 및 공격
            if (CheckTargetInRange())
            {
                isChase = false;
                bChargeStart = true;
            }
            if (!isAttack && !doDie)
            {
                if (bChargeStart)
                {
                    // bChargeStart가 true일 때에만 시간을 세도록 조건 추가
                    AttackChargeTime -= Time.deltaTime; // AttackChargeTime을 시간의 흐름에 따라 감소
                    if (AttackChargeTime <= 0f)
                    {
                        bChargeStart = false;
                        AttackChargeTime = 1.5f;
                        StartCoroutine(Attack());
                    }
                }
            }
        }
    }

    bool CheckTargetInRange()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, targetRadius, transform.forward, out hit, targetRange, LayerMask.GetMask("Player")))
        {
            return true;
        }
        return false;
    }

    IEnumerator Attack()
    {
        // 공격 애니메이션 실행
        // anim.SetTrigger("doAttack");

        Debug.Log("공격");
        attackParticle.Play();
        Instantiate(attackArea,gameObject.transform.position,Quaternion.identity);

        isAttack = true;

        yield return new WaitForSeconds(0.7f);
        attackParticle.Stop();
        FreezeMonster();
        yield return new WaitForSeconds(2f);

        if (!doDie)
        {
            isChase = true;
        }

        bChargeStart = false;
        isAttack = false;
    }

    private void ChoiceStart()
    {
        isChase = true;
    }

   
    IEnumerator Die()
    {
        // 죽음 애니메이션
        // anim.SetTrigger("doDie");

        doDie = true;
        collider.enabled = false;
        isChase = false;
        nav.isStopped = true; // NavMeshAgent 멈춤
        nav.speed = 0;
        nav.angularSpeed = 0;

        FreezeMonster();
        FixPosition(transform.position);

        yield return new WaitForSeconds(2f); // 죽음 애니메이션 속도에 따라 수치 수정
        Destroy(gameObject);
    }



    public void FixPosition(Vector3 desiredPosition)
    {
        transform.position = desiredPosition;
    }
    public void FreezeMonster()
    {
        nav.velocity = Vector3.zero;  // NavMeshAgent의 이동 속도 초기화
        rb.velocity = Vector3.zero;   // Rigidbody의 이동 속도 초기화
        rb.angularVelocity = Vector3.zero;  // Rigidbody의 각속도 초기화
    }


    private void OnDestroy()
    {
        stagemanager.smallNum++;
    }

    public void TakeDamage(Damage damage)
    {
        bChargeStart = false;
        AttackChargeTime = 1.5f;
        isChase = false;
        Invoke("ChoiceStart()", 0.1f);

        if (currentHp <= 0)
        {
            StartCoroutine(Die());

            Die();
        }
        currentHp--;
    }
}



