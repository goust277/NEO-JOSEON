using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemy : MonoBehaviour
{
    private bool hasDied = false;

    [Header("몬스터 정보")]
    public int maxHp;
    public int currentHp;

  
    public virtual void TakeDamage(int damage)
    {

    }

    public virtual void Die()
    {
        if (hasDied) return;  // 이미 실행된 경우 종료
        hasDied = true;
    }



}
