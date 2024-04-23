using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Monster_Jumper : MonoBehaviour, IDamageable
{
    private StageManagerAssist stagemanager;

    [Header("���� ����")]
    public int currentHp;

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

    [Header("�ִϸ��̼� / �ݶ��̴�")]
    public Animator anim;
    public new BoxCollider collider;
    private Transform player; // ��ǥ�� �ϴ� �÷��̾� ��ġ
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
        Debug.Log("����");
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

        // ��Ȯ�ϰ� ��ǥ �������� ȸ�� �ϷḦ �����ϱ� ���� ȸ�� ������ �����մϴ�.
        transform.LookAt(targetPosition);

        yield return new WaitForSeconds(0.2f);

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
        bAttackReady = true;
        Vector3 jumpDirection = transform.up * jumpForce + transform.forward * forwardForce; // ���ʰ� �������� ���� ����
        rb.AddForce(jumpDirection, ForceMode.Impulse); // ���� ���� ����

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
        // ���� �ִϸ��̼�
        // anim.SetTrigger("doDie");


        StopCoroutine(coroutine);
        doDie = true;
        collider.enabled = false;

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
        rb.velocity = Vector3.zero;   // Rigidbody�� �̵� �ӵ� �ʱ�ȭ
        rb.angularVelocity = Vector3.zero;  // Rigidbody�� ���ӵ� �ʱ�ȭ
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Plane"))
        {
            if(bAttackReady)
            {
                Debug.Log("����");
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
