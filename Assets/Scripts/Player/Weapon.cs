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
    }

    IEnumerator Swing()
    {
        if (attackLv == 0 || attackLv == 2)
        {
            yield return GameDefine.waitForSeconds0_1;
            if (attackLv == 2)
            {
                attackLv = 0;
            }
            Attack();
            atk1.SetActive(true);
            Invoke("OffEffect1", 0.5f);
            float delay = 0;
            while (delay < atkDelay)
            {
                delay += Time.deltaTime;
                yield return null;
            }
            BoxCollider.enabled = true;

        }
        else if (attackLv == 1)
        {
            yield return GameDefine.waitForSeconds0_1;
            Attack();
            atk2.SetActive(true);
            Invoke("OffEffect2", 0.5f);
            float delay = 0;
            while (delay < atkDelay)
            {
                delay += Time.deltaTime;
                yield return null;
            }
            BoxCollider.enabled = true;

        }
        //else if (attackLv == 2)
        //{
        //    yield return GameDefine.waitForSeconds0_1;
        //    Attack();
        //    float delay = 0;
        //    while (delay < atkDelay)
        //    {
        //        delay += Time.deltaTime;
        //        yield return null;
        //    }
        //    BoxCollider.enabled = true;
        //    float atk = 0;
        //    while (atk < 0.2f)
        //    {
        //        atk += Time.deltaTime;
        //        yield return null;
        //    }
        //    BoxCollider.enabled = false;

        //}
        float onAttack = 0;
        while (onAttack < rate)
        {
            onAttack += Time.deltaTime;
            yield return null;
        }
        BoxCollider.enabled = false;
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
            if (target == null) return; // 없다면 리턴

            StartCoroutine(HitLag());

            Damage d; // 대미지 구조체
            d.amount = damage; // 피해량
            d.property = string.Empty; // 속성
            target.TakeDamage(d); // '피해 받기' 메서드 호출
        }
    }


    IEnumerator HitLag()
    {
        Time.timeScale = 0.2f;

        yield return new WaitForSecondsRealtime(0.05f);

        Time.timeScale = 1f;
    }
}
