using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.WSA;

public class Weapon : MonoBehaviour
{
    private BoxCollider BoxCollider;
    public enum Type { Melee, Range };
    public Type type;

    public int attackLv = 0;

    public int damage;
    public float rate;
    public float atkDelay = 0.3f;

    public bool isAtkTime;

    public float effectTime;
    [SerializeField] private GameObject effect;

    [SerializeField] private GameObject atk1;
    [SerializeField] private GameObject atk2;
    

    private void Start()
    {
        BoxCollider = GetComponent<BoxCollider>();
        BoxCollider.enabled = false;
        atk1.SetActive(false);
        atk2.SetActive(false);
    }
    public void Use()
    {
        if (type == Type.Melee) 
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
    }

    public void TakeDamage()
    {
        StopCoroutine("Swing");  
        isAtkTime = false;
        attackLv = 0;
    }

    IEnumerator Swing()
    {
        if (attackLv == 0 || attackLv == 2)
        {
            BoxCollider.enabled = true;
            if (attackLv == 2)
            {
                attackLv = 0;
            }
            Attack();
            
            atk1.SetActive(true);
            Invoke("OffEffect1", 0.1f);

        }
        else if (attackLv == 1)
        {
            BoxCollider.enabled = false;
            Attack();
            BoxCollider.enabled = true;
            atk2.SetActive(true);
            Invoke("OffEffect2", 0.1f);

            yield return new WaitForSeconds(0.1f);
            BoxCollider.enabled = false;

        }
        float onAttack = 0;
        while (onAttack < rate)
        {
            onAttack += Time.deltaTime;
            yield return null;
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
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable target = other.GetComponent<IDamageable>(); // 인터페이스 찾기
            Debug.Log(target);
            if (target == null) return; // 없다면 리턴

            StartCoroutine(HitLag());
            Debug.Log("1");
            Damage d; // 대미지 구조체
            d.amount = damage; // 피해량
            d.property = string.Empty; // 속성
            target.TakeDamage(d); // '피해 받기' 메서드 호출


        }

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

        yield return new WaitForSecondsRealtime(0.05f);

        Time.timeScale = 1f;
    }
}
