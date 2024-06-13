using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster_Sangmo : NewEnemy
{
    private StageManagerAssist stagemanager;

    [Header("몬스터 상태 / 관련 오브젝트")]
    public bool doDie;
    public bool isAttack;
    public bool isTakeDamage; // 데미지를 받고 있지 않은 상황 + 물살이 있는 상황에서만 공격
    public bool isInWater;

    [Header("공격")]
    public GameObject AttackArea;


    [Header("애니메이션 / 콜라이더")]
    public Animator anim;
    public new Collider collider;
    private Rigidbody rb;

    [Header("머테리얼")]
    private Material[] originalMaterials;
    Renderer[] renderers;
    public Material white;
    public Material black;

    [Header("기타 오브젝트")]
    private Transform player; // 목표로 하는 플레이어 위치

    [Header("이펙트")]
    public GameObject destroyParticle;
    public ParticleSystem particleAttack;
    [SerializeField] private ParticleSystem hitEffect;
    private float interval = 0.3f;
    private bool isInvoking = false;

    [Header("체력바")]
    public GameObject HpBar;
    public Image ImageHp;

    public Transform positionNumBox;
    public GameObject DmgNumBox;

    private void Start()
    {
        stagemanager = FindObjectOfType<StageManagerAssist>();


        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();

        // #. 머테리얼 찾아오기
        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++) originalMaterials[i] = renderers[i].material;
    }


    private void Update()
    {
        if(!isTakeDamage && isInWater)
        {
            anim.SetBool("isAttack", true);
            AttackArea.SetActive(true);
            if (!isInvoking)
            {
                InvokeRepeating("PlayParticleAttack", 0f, interval);
                isInvoking = true;
            }
        }
        else
        {
            anim.SetBool("isAttack", false);
            AttackArea.SetActive(false);
            if (isInvoking)
            {
                CancelInvoke("PlayParticleAttack");
                isInvoking = false;
            }
        }
    }
    private void PlayParticleAttack()
    {
        particleAttack.Play();
    }



    public override void TakeDamage(int damage)
    {
        if (!doDie)
        {
            Debug.Log("데미지 입음");

            int tempDmgNum = damage * Random.Range(80, 120);
            currentHp -= tempDmgNum;
            float remainingHpPercentage = Mathf.Round(((float)currentHp / (float)maxHp) * 100f) / 100f;
            ImageHp.fillAmount = remainingHpPercentage;

            Vector3 parentForward = positionNumBox.forward;
            Quaternion rotation_ = Quaternion.LookRotation(parentForward);
            Quaternion yRotation = Quaternion.Euler(0, 180, 0);
            Quaternion finalRotation = rotation_ * yRotation;
            GameObject dmgNumbobbox = Instantiate(DmgNumBox, positionNumBox.position, finalRotation);
            DmgNum dmgBox = dmgNumbobbox.GetComponent<DmgNum>();
            dmgBox.text_dmgNum.text = tempDmgNum.ToString();

            hitEffect.Play();

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
            StartCoroutine(Die_());
        }
        else
        {
            isTakeDamage = true;
            anim.SetBool("isAttack", false);

            anim.SetTrigger("doStun");

            StartCoroutine(ChangeMaterials(white, 0.08f));

            Vector3 direction = player.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;

            yield return new WaitForSeconds(0.5f);

            isTakeDamage = false;

        }
        yield return new WaitForSeconds(0f);
       
    }
    IEnumerator ChangeMaterials(Material newMaterial, float duration)
    {
        foreach (Renderer renderer in renderers) renderer.material = newMaterial;
        yield return new WaitForSeconds(duration);
        for (int i = 0; i < renderers.Length; i++) renderers[i].material = originalMaterials[i];
    }

    public override void Die()
    {
        base.Die();
        StartCoroutine(Die_());
    }
    IEnumerator Die_()
    {
        ChangeMaterialsBlack(black);
        HpBar.SetActive(false);

        anim.SetTrigger("doDie");

        collider.enabled = false;


        FreezeMonster();
        FixPosition(transform.position);

        yield return new WaitForSeconds(2f); // 죽음 애니메이션 속도에 따라 수치 수정
        Vector3 spawnPosition = transform.position + Vector3.up; // 현재 위치에서 위로 1만큼 이동한 위치 계산
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
        rb.velocity = Vector3.zero;   // Rigidbody의 이동 속도 초기화
        rb.angularVelocity = Vector3.zero;  // Rigidbody의 각속도 초기화
        rb.constraints = RigidbodyConstraints.FreezePositionY;
    }


    private void OnDestroy()
    {
        stagemanager.smallNum++;
    }



}
