using Cinemachine;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.WSA;

public class Weapon : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook CMfl;
    private CinemachineBasicMultiChannelPerlin ChannelPerlin;

    public float ShakeDuration = 0.3f;
    public float ShakeAmplitude = 1.2f;
    public float ShakeFrequency = 0.4f;

    private Coroutine coroutine;
    private MeshCollider BoxCollider;

    [SerializeField] private Animator animator;
    public enum Type { Melee, Range };
    public Type type;

    public int attackLv = 0;

    public int damage;
    public float rate;

    public bool isAtkTime;

    public float effectTime;
    [SerializeField] private GameObject effect;

    [SerializeField] private GameObject atk1;
    [SerializeField] private GameObject atk2;
    

    private void Start()
    {
        if (CMfl != null)
        {
            ChannelPerlin = CMfl.GetComponent<CinemachineBasicMultiChannelPerlin>();
   
        }
            
        BoxCollider = GetComponent<MeshCollider>();
        BoxCollider.enabled = false;
        atk1.SetActive(false);
        atk2.SetActive(false);
    }
    public void Use()
    {
        if (type == Type.Melee) 
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            coroutine = StartCoroutine(Swing());
        }
    }

    public void StopAtk()
    {
        if (atk1)
        {
            atk1.SetActive (false);
        }
        else if (atk2) 
        {
            atk2.SetActive(false);
        }
        if (coroutine != null) 
        {
            StopCoroutine(coroutine);
        }
 
        isAtkTime = false;
        attackLv = 0;
    }
    
    
    IEnumerator Swing()
    {
        if (attackLv == 0 || attackLv == 2)
        {
            if (atk2)
            {
                atk2.SetActive(false);
            }
            BoxCollider.enabled = true;
            if (attackLv == 2)
            {
                attackLv = 0;
            }
            Attack();
            animator.SetTrigger("Atk1");
            atk1.SetActive(true);
            Invoke("OffEffect1", 0.3f);
            yield return new WaitForSeconds(rate + 0.1f);

        }
        else if (attackLv == 1)
        {
            if (atk1)
            {
                atk1.SetActive(false);
            }
            animator.SetTrigger("Atk2");
            BoxCollider.enabled = false;
            Attack();
            BoxCollider.enabled = true;
            atk2.SetActive(true);
            Invoke("OffEffect2", 0.3f);

            yield return new WaitForSeconds(0.1f);
            BoxCollider.enabled = false;
            yield return new WaitForSeconds(rate);

        }
        

        isAtkTime = false;
        attackLv = 0;
    }
    private void OffEffect1()
    {
        atk1.SetActive(false);
    }
    private void OffEffect2()
    {
        atk2.SetActive(false);
    }
    private void Attack()
    {
        isAtkTime = true;
        attackLv++;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))
        {
            Vector3 triggerPosition = other.ClosestPoint(transform.position);
            Debug.Log("Enter");

            if (effect != null)
            {
                GameObject spawnEffect = Instantiate(effect, triggerPosition, Quaternion.identity);
                Destroy(spawnEffect, effectTime);
            }
            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<NewEnemy>().TakeDamage(damage);

                CMfl.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = ShakeFrequency;
                CMfl.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = ShakeFrequency;
                CMfl.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = ShakeFrequency;
                StartCoroutine(HitLag());

                Invoke("Zero", 0.1f);
            }
        }

    }

    private void Zero()
    {
        CMfl.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
        CMfl.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
        CMfl.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
    }
    public void AttOn()
    {
        Debug.Log("1");
        BoxCollider.enabled = true;
    }
    public void AttkOff()
    {
        BoxCollider.enabled = false;
    }
    IEnumerator HitLag()
    {
        Time.timeScale = 0.2f;

        yield return new WaitForSecondsRealtime(0.1f);

        Time.timeScale = 1f;
    }
}
