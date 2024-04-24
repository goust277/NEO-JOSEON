using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Monster_Boomer : MonoBehaviour ,IDamageable
{
    private StageManagerAssist stagemanager;

    [Header("���� ����")]
    public int currentHp;

    [Header("���� ���� / ���� ������Ʈ")]
    public bool isChase = true;
    public bool isAttack;
    public bool doDie;

    [Header("���� ����")]
    public GameObject attackArea;
    public float targetRadius;
    public float targetRange;
    public float AttackChargeTime;
    public bool bChargeStart;

    [Header("����Ʈ")]
    public ParticleSystem attackParticle;

    [Header("�ִϸ��̼� / �ݶ��̴�")]
    public Animator anim;
    public new BoxCollider collider;
    private Transform player; // ��ǥ�� �ϴ� �÷��̾� ��ġ
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
                nav.isStopped = true; // NavMeshAgent ����
                nav.speed = 0;
                nav.angularSpeed = 0;
                FreezeMonster();
            }
            else
            {
                nav.isStopped = false; // NavMeshAgent ����
                nav.speed = 3f;
                nav.angularSpeed = 600;
            }

            // Ÿ���� �� ����
            if (CheckTargetInRange())
            {
                isChase = false;
                bChargeStart = true;
            }
            if (!isAttack && !doDie)
            {
                if (bChargeStart)
                {
                    // bChargeStart�� true�� ������ �ð��� ������ ���� �߰�
                    AttackChargeTime -= Time.deltaTime; // AttackChargeTime�� �ð��� �帧�� ���� ����
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
        // ���� �ִϸ��̼� ����
        // anim.SetTrigger("doAttack");

        Debug.Log("����");
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
        // ���� �ִϸ��̼�
        // anim.SetTrigger("doDie");

        doDie = true;
        collider.enabled = false;
        isChase = false;
        nav.isStopped = true; // NavMeshAgent ����
        nav.speed = 0;
        nav.angularSpeed = 0;

        FreezeMonster();
        FixPosition(transform.position);

        yield return new WaitForSeconds(2f); // ���� �ִϸ��̼� �ӵ��� ���� ��ġ ����
        Destroy(gameObject);
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



