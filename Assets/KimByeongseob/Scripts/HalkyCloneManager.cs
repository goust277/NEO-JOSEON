using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HalkyCloneManager : NewEnemy
{
    private SoundManager soundManager;
    public GameObject[] sangmoObject;       // ��� ������Ʈ
    public float slapTime = 0.1f;         // �ķ�ġ�� �ǰ�Ÿ��
    public float spinTime = 2f;           // ��� ������ �ǰ�Ÿ��
    public float S_initialDelay = 0.7f;   // ����
    public float S_endingDelay = 1.2f;    // �ĵ�
    public float spinSpeed = 30f;         // ��� ������ �ӵ�
    public int slapDamage = 5;     // �ķ�ġ�� ������
    public int spinDamage = 4;     // ��� ������ ������

    public GameObject shield; // ���� ������Ʈ

    private Animator animator; // �ִϸ����� ������Ʈ
    private Transform playerTransform;

    public ParticleSystem effectAttack1;
    public ParticleSystem effectAttack2;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        soundManager = FindObjectOfType<SoundManager>();
    }

    private void OnEnable()
    {
        // ��� ������Ʈ�� ��Ȱ��ȭ
        foreach (var obj in sangmoObject)
        {
            obj.SetActive(false);
        }
    }


    // �ٰŸ� ���� (�ķ�ġ��, ��� ������)
    public IEnumerator StartShortAttack(int attackType)
    {
        switch (attackType)
        {
            case 1:
                // �ķ�ġ�� ����
                animator.SetTrigger("IsShortAttack_1");
                yield return new WaitForSeconds(S_initialDelay);
                soundManager.PlaySound("Attack_1");
                transform.LookAt(playerTransform);
                sangmoObject[0].SetActive(true);
                effectAttack1.Play();
                yield return new WaitForSeconds(slapTime);
                sangmoObject[0].SetActive(false);
                effectAttack1.Stop();
                yield return new WaitForSeconds(S_endingDelay);
                break;
            case 2:
                // ��� ������ ����
                float elapsedTime = 0f;
                float toggleInterval = 0.2f; // ����Ʈ�� ���� �״� �� ��
                transform.LookAt(playerTransform);
                soundManager.StartLoopSound("Attack_2");
                animator.SetTrigger("IsShortAttack_2");
                yield return new WaitForSeconds(S_initialDelay);
                sangmoObject[1].SetActive(true);
                while (elapsedTime < spinTime)
                {
                    effectAttack2.Play();
                    yield return new WaitForSeconds(toggleInterval);
                    effectAttack2.Stop();
                    yield return new WaitForSeconds(toggleInterval);
                    elapsedTime += 2 * toggleInterval;
                }
                sangmoObject[1].SetActive(false);
                soundManager.StopLoopSound("Attack_2");
                yield return new WaitForSeconds(S_endingDelay);
                break;
        }
    }

    public override void TakeDamage(int damage)
    {
        StartCoroutine(ActivateShield());
    }

    // ���� Ȱ��ȭ �ڷ�ƾ
    private IEnumerator ActivateShield()
    {
        if (shield != null)
        {
            shield.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            shield.SetActive(false);
        }
    }
}
