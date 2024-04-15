using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEditor.UIElements;
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
    

    private void Start()
    {
        BoxCollider = GetComponent<BoxCollider>();
        BoxCollider.enabled = false;
    }
    public void Use()
    {
        if (type == Type.Melee) 
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
    }

    IEnumerator Swing()
    {
        if (attackLv == 0 || attackLv == 3)
        {
            yield return GameDefine.waitForSeconds0_1;
            if (attackLv == 3)
            {
                attackLv = 0;
            }
            Attack();
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
            float delay = 0;
            while (delay < atkDelay)
            {
                delay += Time.deltaTime;
                yield return null;
            }
            BoxCollider.enabled = true;

        }
        else if (attackLv == 2)
        {
            yield return GameDefine.waitForSeconds0_1;
            Attack();
            float delay = 0;
            while (delay < atkDelay)
            {
                delay += Time.deltaTime;
                yield return null;
            }
            BoxCollider.enabled = true;

        }
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

    private void Attack()
    {

        isAtkTime = true;
        attackLv++;
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable target = other.GetComponent<IDamageable>(); // 인터페이스 찾기
        if (target == null) return; // 없다면 리턴

        Time.timeScale = 0.1f;

        Damage d; // 대미지 구조체
        d.amount = damage; // 피해량
        d.property = string.Empty; // 속성
        target.TakeDamage(d); // '피해 받기' 메서드 호출
    }
}
