using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Monster_Jumper : NewEnemy
{
    private StageManagerAssist stagemanager;

    [Header("���� ���� / ���� ������Ʈ")]
    public bool isAttack;
    public bool isChase;
    public bool doDie;

    [Header("���� ����")]
    public GameObject attackArea;
    public float jumpForce;
    public float forwardForce;
    public bool bAttackReady;

    [Header("����Ʈ")]
    public ParticleSystem attackParticle;
    public GameObject destroyParticle;

    [Header("�ִϸ��̼� / �ݶ��̴�")]
    public Animator anim;
    public new Collider collider;
    private Transform player; // ��ǥ�� �ϴ� �÷��̾� ��ġ
    private Rigidbody rb;

    [Header("���׸���")]
    public Renderer renderer;
    public Material[] materials;
    public Material white;
    public Material black;
    private Material[] originalMaterials;

    private Coroutine coroutine;

    [Header("ü�¹�")]
    public GameObject HpBar;
    public Image ImageHp;

    public Transform positionNumBox;
    public GameObject DmgNumBox;

    void Start()
    {
        stagemanager = FindObjectOfType<StageManagerAssist>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();

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
        rb.isKinematic = true;

        Debug.Log("����");
        attackParticle.Play();
        GameObject newAttackArea = Instantiate(attackArea, gameObject.transform.position, Quaternion.identity);
        Transform newAttackAreaTransform = newAttackArea.transform;
        newAttackAreaTransform.localScale = new Vector3(4f, 4f, 4f);
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
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, (elapsedTime / duration) * 3f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);

        yield return StartCoroutine(Jump(jumpForce, forwardForce)); // Jump �ڷ�ƾ�� ȣ���ϰ� �� ������ �Ϸ�� ������ ��ٸ��ϴ�.
    }
    IEnumerator Jump(float jumpHeight, float jumpDuration)
    {
        // ������ ���� ���� �ڷ�ƾ�� ����ϴ�.
        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(JumpCoroutine(jumpHeight, jumpDuration));
        yield return coroutine;
    }
    IEnumerator JumpCoroutine(float jumpHeight, float jumpDuration)
    {
        anim.SetTrigger("doJump");
        rb.isKinematic = false;
        yield return new WaitForSeconds(0.3f);
        bAttackReady = true;
        Vector3 jumpDirection = transform.up * jumpForce + transform.forward * forwardForce; // ���ʰ� �������� ���� ����
        rb.AddForce(jumpDirection, ForceMode.Impulse);

        //yield return new WaitForSeconds(1.0f);
        //anim.SetTrigger("doDie");

        yield return new WaitForSeconds(5.0f);

        isChase = false;
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


    public override void TakeDamage(int damage)
    {
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

        if (currentHp <= 0)
        {
            StartCoroutine(Die_());
        }
        else
        {
            ChangeMaterials(white);

            Vector3 direction = player.position - transform.position;
            Quaternion _rotation = Quaternion.LookRotation(direction);
            transform.rotation = _rotation;
        }
    }

    public override void Die()
    {
        base.Die();
        StartCoroutine(Die_());

    }
    IEnumerator Die_()
    {
        StopCoroutine(coroutine);
        HpBar.SetActive(false);

        for (int i = 0; i < materials.Length; i++) materials[i] = black;
        renderer.materials = materials;

        anim.SetTrigger("doDie");


        doDie = true;
        collider.enabled = false;

        FreezeMonster();
        FixPosition(transform.position);

        yield return new WaitForSeconds(0.7f); // ���� �ִϸ��̼� �ӵ��� ���� ��ġ ����
        Vector3 spawnPosition = transform.position + Vector3.up; // ���� ��ġ���� ���� 1��ŭ �̵��� ��ġ ���
        Instantiate(destroyParticle, spawnPosition, Quaternion.identity);
        Destroy(gameObject);
    }

    void ChangeMaterials(Material newMaterial)
    {
        for (int i = 0; i < materials.Length; i++) materials[i] = newMaterial;
        renderer.materials = materials;
        Invoke("RestoreMaterials", 0.08f);
    }
    void RestoreMaterials()
    {
        for (int i = 0; i < materials.Length; i++) materials[i] = originalMaterials[i];
        renderer.materials = materials;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Plane"))
        {
            if (bAttackReady)
            {
                anim.SetTrigger("doLand");
                StartCoroutine(Attack());
            }
        }
    }

    private void OnDestroy()
    {
        stagemanager.smallNum++;
    }
}
