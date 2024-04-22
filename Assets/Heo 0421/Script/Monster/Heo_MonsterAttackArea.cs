using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heo_MonsterAttackArea : MonoBehaviour
{
    public float fDamage;

    private void Awake()
    {
        Destroy(gameObject, 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 데미지 입음");
            // 데미지 처리 
        }
    }
}
