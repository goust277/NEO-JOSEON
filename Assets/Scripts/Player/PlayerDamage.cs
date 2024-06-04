using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

    [Header("사망 화면")]
    [SerializeField] private GameObject die;
    [SerializeField] private GameObject diePanel;
    private Image dieImg;
    [SerializeField]private float temp = 0;
    private Rigidbody rb;
    private BoxCollider boxCollider;

    void Start()
    {
        CurrentHp = MaxHp;

        animator = GetComponent<Animator>();
        hit = GetComponent<PlayerMove>();
        if (die != null && diePanel != null )
        {
            dieImg = die.GetComponent<Image>();

            rb = GetComponent<Rigidbody>();
            die.SetActive(false);
            diePanel.SetActive(false);

        }

        boxCollider = GetComponent<BoxCollider>();


        if (CMfl != null)
        {
            ChannelPerlin = CMfl.GetComponent<CinemachineBasicMultiChannelPerlin>();

        }

        if(Time.timeScale == 0f)
        {

            Time.timeScale = 1f;
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
            hit.Ondie = true;
            boxCollider.enabled = false;
            rb.useGravity = false;
            Invoke("Die", 2f);
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

        if (CurrentHp <= 0)
        {
            animator.SetTrigger("Death");
            die.SetActive(true);
        }
    }


    IEnumerator Hit()
    {
        CMfl.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = ShakeFrequency;
        CMfl.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = ShakeFrequency;
        CMfl.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = ShakeFrequency;
        isHit = true;

        yield return new WaitForSecondsRealtime(0.2f);
        CMfl.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
        CMfl.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
        CMfl.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;

        yield return new WaitForSecondsRealtime(0.2f);
        isHit = false;
    }

    private void Die()
    {

        Color tempColor = dieImg.color;

        if (temp <= 3 )
        {
            temp += Time.deltaTime;
            tempColor.a = temp;

            dieImg.color = tempColor;
        }
        else if (temp >= 3)
        {
            diePanel.SetActive(true);
            Time.timeScale = 0f;
            hit.MouseOn();
        }
        
    }
}
