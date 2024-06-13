using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HalkyCloneManager : NewEnemy
{
    private SoundManager soundManager;
    public GameObject[] sangmoObject;       // 상모 오브젝트
    public float slapTime = 0.1f;         // 후려치기 피격타임
    public float spinTime = 2f;           // 상모 돌리기 피격타임
    public float S_initialDelay = 0.7f;   // 선딜
    public float S_endingDelay = 1.2f;    // 후딜
    public float spinSpeed = 30f;         // 상모 돌리기 속도
    public int slapDamage = 5;     // 후려치기 데미지
    public int spinDamage = 4;     // 상모 돌리기 데미지

    public GameObject shield; // 쉴드 오브젝트

    private Animator animator; // 애니메이터 컴포넌트
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
        // 상모 오브젝트를 비활성화
        foreach (var obj in sangmoObject)
        {
            obj.SetActive(false);
        }
    }


    // 근거리 공격 (후려치기, 상모 돌리기)
    public IEnumerator StartShortAttack(int attackType)
    {
        switch (attackType)
        {
            case 1:
                // 후려치기 공격
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
                // 상모 돌리기 공격
                float elapsedTime = 0f;
                float toggleInterval = 0.2f; // 이펙트를 껐다 켰다 할 간
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

    // 쉴드 활성화 코루틴
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
