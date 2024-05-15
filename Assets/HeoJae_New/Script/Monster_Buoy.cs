using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster_Buoy : NewEnemy
{
    private StageManagerAssist stagemanager;

    [Header("���� ���� / ���� ������Ʈ")]
    public bool doDie;

    [Header("�ִϸ��̼� / �ݶ��̴�")]
    public Animator anim;
    public new Collider collider;
    private Rigidbody rb;

    [Header("���׸���")]
    private Material[] originalMaterials;
    Renderer[] renderers;
    public Material white;
    public Material black;

    [Header("��Ÿ ������Ʈ")]
    private Transform player; // ��ǥ�� �ϴ� �÷��̾� ��ġ

    [Header("����Ʈ")]
    public GameObject destroyParticle;

    [Header("ü�¹�")]
    public GameObject HpBar;
    public Image ImageHp;

    public Transform positionNumBox;
    public GameObject DmgNumBox;


    private void Start()
    {
        stagemanager = FindObjectOfType<StageManagerAssist>();


        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();

        // #. ���׸��� ã�ƿ���
        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++) originalMaterials[i] = renderers[i].material;
    }


    public override void TakeDamage(int damage)
    {
        if (!doDie)
        {
            Debug.Log("������ ����");

            int tempDmgNum = damage * Random.Range(80, 120);
            currentHp -= tempDmgNum;
            float remainingHpPercentage = Mathf.Round(((float)currentHp / (float)maxHp) * 100f) / 100f;
            ImageHp.fillAmount = remainingHpPercentage;

            Vector3 parentForward = positionNumBox.forward;
            Quaternion rotation__ = Quaternion.LookRotation(parentForward);
            GameObject dmgNumbobbox = Instantiate(DmgNumBox, positionNumBox.position, rotation__);
            DmgNum dmgBox = dmgNumbobbox.GetComponent<DmgNum>();
            dmgBox.text_dmgNum.text = tempDmgNum.ToString();



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
            anim.SetTrigger("doStun");

            StartCoroutine(ChangeMaterials(white, 0.08f));

            Vector3 direction = player.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;


        }
        yield return new WaitForSeconds(0f);
    }
    IEnumerator ChangeMaterials(Material newMaterial, float duration)
    {
        foreach (Renderer renderer in renderers) renderer.material = newMaterial;
        yield return new WaitForSeconds(duration);
        for (int i = 0; i < renderers.Length; i++) renderers[i].material = originalMaterials[i];
    }


    IEnumerator Die()
    {
        ChangeMaterialsBlack(black);
        HpBar.SetActive(false);

        anim.SetTrigger("doDie");

        collider.enabled = false;


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
        rb.velocity = Vector3.zero;   // Rigidbody�� �̵� �ӵ� �ʱ�ȭ
        rb.angularVelocity = Vector3.zero;  // Rigidbody�� ���ӵ� �ʱ�ȭ
        rb.constraints = RigidbodyConstraints.FreezePositionY;
    }




    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerDamage>().TakeDamage();
            return;
        }
    }

}