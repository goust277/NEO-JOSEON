using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.XR.WSA;

public class Weapon : MonoBehaviour
{

    [SerializeField] private Material[] matarials;

    private Renderer rend;
    private CapsuleCollider capsuleCollider;
    public enum Type { Melee, Range };
    public Type type;

    public int attackLv = 0;

    public int damage;
    public float rate;

    public bool isAtkTime;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleCollider.enabled = false;
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
            Attack();
        }
        else if (attackLv == 1)
        {
            yield return GameDefine.waitForSeconds0_1;
            Attack();
        }
        else if (attackLv == 2)
        {
            yield return GameDefine.waitForSeconds0_1;
            Attack();
        }
        float onAttack = 0;
        while (onAttack < rate)
        {
            onAttack += Time.deltaTime;
            yield return null;
        }
        capsuleCollider.enabled = false;
        rend.material = matarials[0];
        yield return new WaitForSeconds(0.2f);
        isAtkTime = false;
        attackLv = 0;
    }

    private void Attack()
    {
        capsuleCollider.enabled = true;
        rend.material = matarials[1];
        isAtkTime = true;
        attackLv++;
    }

    void attackRester()
    {
        attackLv = 0;
    }
}
