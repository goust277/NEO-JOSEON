using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Monster_Boomer : NewEnemy
{
    private StageManagerAssist stagemanager;
    private AudioSource audioSource;



    [Header("����")]
    [SerializeField] private AudioClip atk;
    [SerializeField] private AudioClip die;
    [SerializeField] private AudioClip walk;

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
    public GameObject destroyParticle;
    [SerializeField] private ParticleSystem hitEffect;

    [Header("�ִϸ��̼� / �ݶ��̴�")]
    public Animator anim;
    public new Collider collider;
    private Transform player; // ��ǥ�� �ϴ� �÷��̾� ��ġ
    private Rigidbody rb;
    private NavMeshAgent nav;

    [Header("���׸���")]
    public Renderer renderer;
    public Material[] materials;
    public Material white;
    public Material black;
    private Material[] originalMaterials;


    private bool bAttackAnim;

    [Header("ü�¹�")]
    public GameObject HpBar;
    public Image ImageHp;

    public Transform positionNumBox;
    public GameObject DmgNumBox;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        stagemanager = FindObjectOfType<StageManagerAssist>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();

        // ���׸��� ���� ����
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
                anim.SetBool("isWalk", false);
                FixPosition(transform.position);
                nav.isStopped = true; // NavMeshAgent ����
                nav.speed = 0;
                nav.angularSpeed = 0;
                FreezeMonster();
            }
            else
            {
                anim.SetBool("isWalk", true);
                nav.isStopped = false;
                nav.speed = 4f;
                nav.angularSpeed = 1200;
                bAttackAnim = false;
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

                    if (!bAttackAnim)
                    {
                        Debug.Log("�ִϸ��̼� �۵�");
                        bAttackAnim = true;
                        anim.SetTrigger("doAttack");
                    }

                    if (AttackChargeTime <= 0f)
                    {
                        bChargeStart = false;
                        AttackChargeTime = 1.0f;
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
        attackParticle.Play();
        GameObject newAttackArea = Instantiate(attackArea, gameObject.transform.position, Quaternion.identity);
        Transform newAttackAreaTransform = newAttackArea.transform;
        newAttackAreaTransform.localScale = new Vector3(4.5f, 4.5f, 4.5f);
        audioSource.clip = atk;
        audioSource.Play();
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




    public override void Die()
    {
        base.Die();
        StartCoroutine(Die_());
        audioSource.clip = die;
        audioSource.Play();
    }
    IEnumerator Die_()
    {
        for (int i = 0; i < materials.Length; i++) materials[i] = black;
        renderer.materials = materials;

        anim.SetTrigger("doDie");
        HpBar.SetActive(false);

        bChargeStart = false;
        AttackChargeTime = 1f;

        collider.enabled = false;
        isChase = false;
        nav.isStopped = true; // NavMeshAgent ����
        nav.speed = 0;
        nav.angularSpeed = 0;

        FreezeMonster();
        FixPosition(transform.position);

        yield return new WaitForSeconds(2f); // ���� �ִϸ��̼� �ӵ��� ���� ��ġ ����
        Vector3 spawnPosition = transform.position + Vector3.up; // ���� ��ġ���� ���� 1��ŭ �̵��� ��ġ ���
        Instantiate(destroyParticle, spawnPosition, Quaternion.identity);
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

    public override void TakeDamage(int damage)
    {
        if (!doDie)
        {
            anim.SetBool("isWalk", false);

            int tempDmgNum = damage * Random.Range(80, 120);
            currentHp -= tempDmgNum;
            float remainingHpPercentage = Mathf.Round(((float)currentHp / (float)maxHp) * 100f) / 100f;
            ImageHp.fillAmount = remainingHpPercentage;

            Vector3 parentForward = positionNumBox.forward;
            Quaternion rotation = Quaternion.LookRotation(parentForward);
            Quaternion yRotation = Quaternion.Euler(0, 180, 0);
            Quaternion finalRotation = rotation * yRotation;
            GameObject dmgNumbobbox = Instantiate(DmgNumBox, positionNumBox.position, finalRotation);
            DmgNum dmgBox = dmgNumbobbox.GetComponent<DmgNum>();
            dmgBox.text_dmgNum.text = tempDmgNum.ToString();

            hitEffect.Play();
            Invoke("StopHit", 0.2f);


            isChase = false;
            ChangeMaterials(white);
            StopAllCoroutines();
            StartCoroutine(TakeDamage());
        }

    }

    private void StopHit()
    {
        hitEffect.Stop();
    }

    IEnumerator TakeDamage()
    {
        if (currentHp <= 0)
        {
            doDie = true;
            StopAllCoroutines();
            StartCoroutine(Die_());
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
            AttackChargeTime = 1f;
            bAttackAnim = false;
        }

        yield return new WaitForSeconds(0.8f);

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



