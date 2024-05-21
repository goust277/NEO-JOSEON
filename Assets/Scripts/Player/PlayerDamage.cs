using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamage : MonoBehaviour
{
    public float MaxHp;
    public float CurrentHp;
    [SerializeField] private float hitDelay;
    public bool isHit = false;
    private float delay = 0f;
    private bool isHitPosible = true;
    private Animator animator;
    private PlayerMove hit;

    [Header("카메라")]
    [SerializeField] private CinemachineFreeLook CMfl;
    private CinemachineBasicMultiChannelPerlin ChannelPerlin;
    //public float ShakeDuration = 0.3f;
    //public float ShakeAmplitude = 1.2f;
    [Header("흔들림 강도")]
    public float ShakeFrequency = 0.4f;

    void Start()
    {
        animator = GetComponent<Animator>();
        hit = GetComponent<PlayerMove>();

        CurrentHp = MaxHp;

        if (CMfl != null)
        {
            ChannelPerlin = CMfl.GetComponent<CinemachineBasicMultiChannelPerlin>();

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hitDelay > delay)
        {
            delay += Time.deltaTime;
        }
        else
        {
            isHitPosible = true;
        }
        if(CurrentHp <= 0)
        {
            Destroy(gameObject, 0.5f);
        }
    }

    public void TakeDamage(float damage = 1.0f)
    {
        if (isHitPosible)
        {
            StartCoroutine(Hit());
            isHitPosible = false;
            delay = 0f;
            animator.SetTrigger("Hit");
            hit.TakeDamage();
            CurrentHp -= damage;
        }
    }


    IEnumerator Hit()
    {
        CMfl.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = ShakeFrequency;
        CMfl.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = ShakeFrequency;
        CMfl.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = ShakeFrequency;
        isHit = true;

        yield return new WaitForSecondsRealtime(0.1f);
        CMfl.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
        CMfl.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
        CMfl.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;

        yield return new WaitForSecondsRealtime(0.3f);
        isHit = false;
    }
}
