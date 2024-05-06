using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster_Arrow : NewEnemy
{
    private StageManagerAssist stagemanager;

    [Header("몬스터 상태 / 관련 오브젝트")]
    public bool isAttack;
    public bool doDie;

    [Header("공격 관련")]
    public GameObject arrowObj;
    public Transform positionCreateArrow;
    public float AttackChargeTime;
    public float arrowSpeed;
    private bool bChargeStart;


    [Header("애니메이션 / 콜라이더")]
    public Animator anim;
    public new Collider collider;
    private Rigidbody rb;

    [Header("머테리얼")]
    public Renderer renderer;
    public Material[] materials;
    public Material white;
    public Material black;
    private Material[] originalMaterials;

    [Header("이펙트")]
    public GameObject destroyParticle;

    [Header("기타 오브젝트")]
    private Transform player; // 목표로 하는 플레이어 위치

    private void Start()
    {
        stagemanager = FindObjectOfType<StageManagerAssist>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();

        // 머테리얼 정보 저장
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
        Debug.Log("실행");
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
            Debug.Log("데미지 입음");
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
        rb.velocity = Vector3.zero;   // Rigidbody의 이동 속도 초기화
        rb.angularVelocity = Vector3.zero;  // Rigidbody의 각속도 초기화
    }

    // #.플레이어 방향으로 회전시키는 함수
    public void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0f; // y축 회전을 고려하지 않음
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;
        }
    }
}
