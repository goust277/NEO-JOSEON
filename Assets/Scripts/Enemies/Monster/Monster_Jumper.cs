using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Monster_Jumper : MonoBehaviour, IDamageable
{
    private StageManagerAssist stagemanager;

    [Header("몬스터 정보")]
    public int currentHp;

    [Header("몬스터 상태 / 관련 오브젝트")]
    public bool isAttack;
    public bool isChase;
    public bool doDie;


    [Header("공격 범위")]
    public GameObject attackArea;
    public float jumpForce;
    public float forwardForce;
    public bool bAttackReady;

    [Header("이펙트")]
    public ParticleSystem attackParticle;

    [Header("애니메이션 / 콜라이더")]
    public Animator anim;
    public new BoxCollider collider;
    private Transform player; // 목표로 하는 플레이어 위치
    private Rigidbody rb;

    private Coroutine coroutine;


    void Start()
    {
        stagemanager = FindObjectOfType<StageManagerAssist>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(!doDie)
        {
            if(!isChase)
            {
                isChase = true;
                StartCoroutine(RotateTowardsPlayer(player.position, 1f));
            }
        } 
    }

    IEnumerator Attack()
    {
        Debug.Log("공격");
        attackParticle.Play();
        Instantiate(attackArea, gameObject.transform.position, Quaternion.identity);
        FreezeMonster();

        yield return new WaitForSeconds(0.7f);
        attackParticle.Stop();
        bAttackReady = false;

    }


    IEnumerator RotateTowardsPlayer(Vector3 targetPosition, float duration)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(RotateCoroutine(targetPosition, duration));
        yield return coroutine;
    }
    IEnumerator RotateCoroutine(Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;

        while (elapsedTime < duration)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, (elapsedTime / duration) * 3f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 정확하게 목표 방향으로 회전 완료를 보장하기 위해 회전 각도를 보정합니다.
        transform.LookAt(targetPosition);

        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(Jump(jumpForce, forwardForce)); // Jump 코루틴을 호출하고 그 실행이 완료될 때까지 기다립니다.

    }
    IEnumerator Jump(float jumpHeight, float jumpDuration)
    {
        // 이전에 실행 중인 코루틴을 멈춥니다.
        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(JumpCoroutine(jumpHeight, jumpDuration));
        yield return coroutine;
    }
    IEnumerator JumpCoroutine(float jumpHeight, float jumpDuration)
    {
        bAttackReady = true;
        Vector3 jumpDirection = transform.up * jumpForce + transform.forward * forwardForce; // 위쪽과 앞쪽으로 힘을 가함
        rb.AddForce(jumpDirection, ForceMode.Impulse); // 힘을 가해 점프

        yield return new WaitForSeconds(5.0f);

        isChase = false;
    }



    public void TakeDamage()
    {
        if (currentHp <= 0)
        {
            StartCoroutine(Die());

            Die();
        }
    }

    IEnumerator Die()
    {
        // 죽음 애니메이션
        // anim.SetTrigger("doDie");


        StopCoroutine(coroutine);
        doDie = true;
        collider.enabled = false;

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
        rb.velocity = Vector3.zero;   // Rigidbody의 이동 속도 초기화
        rb.angularVelocity = Vector3.zero;  // Rigidbody의 각속도 초기화
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Plane"))
        {
            if(bAttackReady)
            {
                Debug.Log("실행");
                StartCoroutine(Attack());
            
            }
        }
    }


    private void OnDestroy()
    {
        stagemanager.smallNum++;
    }

    public void TakeDamage(Damage damage)
    {
        currentHp--;
        TakeDamage();
    }
}
