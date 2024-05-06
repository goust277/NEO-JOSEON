using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster_Arrow : NewEnemy
{
    private StageManagerAssist stagemanager;

    [Header("���� ���� / ���� ������Ʈ")]
    public bool isAttack;
    public bool doDie;

    [Header("���� ����")]
    public GameObject arrowObj;
    public Transform positionCreateArrow;
    public float AttackChargeTime;
    public float arrowSpeed;
    private bool bChargeStart;


    [Header("�ִϸ��̼� / �ݶ��̴�")]
    public Animator anim;
    public new Collider collider;
    private Rigidbody rb;

    [Header("���׸���")]
    public Renderer renderer;
    public Material[] materials;
    public Material white;
    public Material black;
    private Material[] originalMaterials;

    [Header("����Ʈ")]
    public GameObject destroyParticle;

    [Header("��Ÿ ������Ʈ")]
    private Transform player; // ��ǥ�� �ϴ� �÷��̾� ��ġ

    private void Start()
    {
        stagemanager = FindObjectOfType<StageManagerAssist>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();

        // ���׸��� ���� ����
        //Material[] objectMaterials = renderer.materials;
        //materials = new Material[objectMaterials.Length];
        //for (int i = 0; i < objectMaterials.Length; i++)
        //{
        //    materials[i] = objectMaterials[i];
        //}

        //originalMaterials = new Material[materials.Length];
        //for (int i = 0; i < materials.Length; i++)
        //{
        //    originalMaterials[i] = materials[i];
        //}
    }

    private void Update()
    {
        if (!doDie)
        {
            LookAtPlayer();


            if (!bChargeStart)
            {
                bChargeStart = true;
            }

            if (bChargeStart)
            {
                AttackChargeTime -= Time.deltaTime;

                if (AttackChargeTime <= 0f)
                {
                   
                    AttackChargeTime = 2f;
                    StartCoroutine(Attack());
                }
            }
        }
    }

    IEnumerator Attack()
    {
        Debug.Log("����");
        ShotArrow();

        yield return new WaitForSeconds(2f);

        bChargeStart = false;

    }
    public void ShotArrow()
    {
        GameObject newSlashObj = Instantiate(arrowObj, positionCreateArrow.position, Quaternion.identity);
        newSlashObj.transform.rotation = transform.rotation;
    }






    public override void TakeDamage(int damage)
    {
        if (!doDie)
        {
            Debug.Log("������ ����");
            currentHp -= damage;
            // ChangeMaterials(white);
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
            // anim.SetTrigger("doStun");

            // Invoke("RestoreMaterials", 0.08f);

            Vector3 direction = player.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;

            bChargeStart = false;
            AttackChargeTime = 3f;
        }
        yield return new WaitForSeconds(0f);
    }
    void ChangeMaterials(Material newMaterial)
    {
        for (int i = 0; i < materials.Length; i++) materials[i] = newMaterial;
        renderer.materials = materials;
    }
    void RestoreMaterials()
    {
        for (int i = 0; i < materials.Length; i++) materials[i] = originalMaterials[i];
        renderer.materials = materials;
    }


    IEnumerator Die()
    {
        for (int i = 0; i < materials.Length; i++) materials[i] = black;
        renderer.materials = materials;

        // anim.SetTrigger("doDie");

        collider.enabled = false;


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
        rb.velocity = Vector3.zero;   // Rigidbody�� �̵� �ӵ� �ʱ�ȭ
        rb.angularVelocity = Vector3.zero;  // Rigidbody�� ���ӵ� �ʱ�ȭ
    }

    // #.�÷��̾� �������� ȸ����Ű�� �Լ�
    public void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0f; // y�� ȸ���� ������� ����
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;
        }
    }
}
