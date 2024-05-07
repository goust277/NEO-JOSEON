using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster_Spear : NewEnemy
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

    [Header("�애니메이션 / 콜라이더")]
    public Animator anim;
    public new Collider collider;
    private Transform player; 
    private Rigidbody rb;
    private NavMeshAgent nav;

    [Header("머테리얼")]
    private Material[] originalMaterials;
    Renderer[] renderers;
    public Material white;
    public Material black;
    
    private bool bAttackAnim;


    void Start()
    {
        stagemanager = FindObjectOfType<StageManagerAssist>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();

        // #. 머테리얼 찾아오기
        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++) originalMaterials[i] = renderers[i].material;
    }

    void Update()
    {
        if (!doDie)
        {
            nav.SetDestination(player.position);


            if (!isChase)
            {
                FixPosition(transform.position);
                nav.isStopped = true; 
                nav.speed = 0;
                nav.angularSpeed = 0;
                FreezeMonster();
            }
            else
            {
                anim.SetBool("isWalk", true);
                nav.isStopped = false;
                nav.speed = 2f;
                nav.angularSpeed = 120;
            }

            if (CheckTargetInRange())
            {
                isChase = false;
                bChargeStart = true;
            }
            if (!isAttack && !doDie)
            {
                if (bChargeStart)
                {
                    anim.SetBool("isWalk", false);
                    AttackChargeTime -= Time.deltaTime; 
                    if (!bAttackAnim)
                    {
                        bAttackAnim = true;
                    }

                    if (AttackChargeTime <= 0f)
                    {
                        isAttack = true;
                        bChargeStart = false;
                        AttackChargeTime = 0.6f;
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
        anim.SetTrigger("doAttack");

        yield return new WaitForSeconds(1.1f);

        attackParticle.Play();
        attackArea.SetActive(true);


        yield return new WaitForSeconds(0.05f);
        attackArea.SetActive(false);

        yield return new WaitForSeconds(0.65f);
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
        ChangeMaterialsBlack(black);

        anim.SetTrigger("doDie");

        bChargeStart = false;
        AttackChargeTime = 1.5f;

        collider.enabled = false;
        isChase = false;
        nav.isStopped = true; 
        nav.speed = 0;
        nav.angularSpeed = 0;

        FreezeMonster();
        FixPosition(transform.position);

        yield return new WaitForSeconds(2f); // ���� �ִϸ��̼� �ӵ��� ���� ��ġ ����
        Vector3 spawnPosition = transform.position + Vector3.up; // ���� ��ġ���� ���� 1��ŭ �̵��� ��ġ ���
        Instantiate(destroyParticle, spawnPosition, Quaternion.identity);
        Destroy(gameObject);
    }
    void ChangeMaterialsBlack(Material newMaterial)
    {
        foreach (Renderer renderer in renderers) renderer.material = newMaterial;
    }



    public void FixPosition(Vector3 desiredPosition)
    {
        transform.position = desiredPosition;
    }
    public void FreezeMonster()
    {
        nav.velocity = Vector3.zero;  // NavMeshAgent�� �̵� �ӵ� �ʱ�ȭ
        rb.velocity = Vector3.zero;   // Rigidbody�� �̵� �ӵ� �ʱ�ȭ
        rb.angularVelocity = Vector3.zero;  // Rigidbody�� ���ӵ� �ʱ�ȭ
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

            StartCoroutine(ChangeMaterials(white, 0.08f));
           

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


    IEnumerator ChangeMaterials(Material newMaterial, float duration)
    {
        foreach (Renderer renderer in renderers) renderer.material = newMaterial;
        yield return new WaitForSeconds(duration);
        for (int i = 0; i < renderers.Length; i++) renderers[i].material = originalMaterials[i];
    }


}
