using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Monster_Boomer : NewEnemy
{
    private StageManagerAssist stagemanager;

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
    public GameObject destroyParticle;

    [Header("애니메이션 / 콜라이더")]
    public Animator anim;
    public new Collider collider;
    private Transform player; // 목표로 하는 플레이어 위치
    private Rigidbody rb;
    private NavMeshAgent nav;

    [Header("머테리얼")]
    public Renderer renderer;
    public Material[] materials;
    public Material white;
    public Material black;
    private Material[] originalMaterials;


    private bool bAttackAnim;


    void Start()
    {
        stagemanager = FindObjectOfType<StageManagerAssist>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();

        // 머테리얼 정보 저장
        Material[] objectMaterials = renderer.materials;
        materials = new Material[objectMaterials.Length];
        for (int i = 0; i < objectMaterials.Length; i++)
        {
            materials[i] = objectMaterials[i];
        }

        originalMaterials = new Material[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalMaterials[i] = materials[i];
        }
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
                anim.SetBool("isWalk", true);
                nav.isStopped = false; 
                nav.speed = 3f;
                nav.angularSpeed = 1200;
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
                    if (!bAttackAnim)
                    {
                        bAttackAnim = true;
                        anim.SetTrigger("doAttack");
                    }
                   
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
        anim.SetBool("isWalk", false);
        attackParticle.Play();
        GameObject newAttackArea = Instantiate(attackArea, gameObject.transform.position, Quaternion.identity);
        Transform newAttackAreaTransform = newAttackArea.transform;
        newAttackAreaTransform.localScale = new Vector3(4.5f, 4.5f, 4.5f); 

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
        bAttackAnim = false;
    }

    

   
    IEnumerator Die()
    {
        for (int i = 0; i < materials.Length; i++) materials[i] = black;
        renderer.materials = materials;

        anim.SetTrigger("doDie");

        bChargeStart = false;
        AttackChargeTime = 1.5f;

        collider.enabled = false;
        isChase = false;
        nav.isStopped = true; // NavMeshAgent 멈춤
        nav.speed = 0;
        nav.angularSpeed = 0;

        FreezeMonster();
        FixPosition(transform.position);

        yield return new WaitForSeconds(2f); // 죽음 애니메이션 속도에 따라 수치 수정
        Vector3 spawnPosition = transform.position + Vector3.up; // 현재 위치에서 위로 1만큼 이동한 위치 계산
        Instantiate(destroyParticle, spawnPosition, Quaternion.identity);
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

    public override void TakeDamage(int damage)
    {
        if (!doDie)
        {
            anim.SetBool("isWalk", false);
            currentHp -= damage;
            isChase = false;
            ChangeMaterials(white);
            StopAllCoroutines();
            StartCoroutine(TakeDamage__());
        }
           
    }

    IEnumerator TakeDamage__()
    {
        if (currentHp <= 0)
        {
            doDie = true;
            StopAllCoroutines();
            StartCoroutine(Die());
        }
        else
        {
            Invoke("RestoreMaterials", 0.08f);

            Vector3 direction = player.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;

            anim.SetTrigger("doStun");

            isAttack = true;
            bChargeStart = false;
            AttackChargeTime = 1.5f;
            bAttackAnim = false;
        }

        yield return new WaitForSeconds(1.2f); 

        isChase = true;
        isAttack = false;
        bChargeStart = false;
        isAttack = false;
        bAttackAnim = false;
    }


    private void ChoiceStart()
    {
        Debug.Log("asdasdasd");

        isChase = true;
        isAttack = false;
        bChargeStart = false;
        isAttack = false;
        bAttackAnim = false;
        anim.SetBool("isWalk", true);
    }

    void ChangeMaterials(Material newMaterial)
    { 
        for (int i = 0; i < materials.Length; i++) materials[i] = newMaterial;
        renderer.materials = materials;
        // Invoke("RestoreMaterials", 0.05f);
    }
    void RestoreMaterials()
    {
        for (int i = 0; i < materials.Length; i++) materials[i] = originalMaterials[i];
        renderer.materials = materials;
    }
}



