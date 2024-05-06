using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemy : MonoBehaviour
{
    [Header("몬스터 정보")]
    public int currentHp;

    public virtual void TakeDamage(int damage)
    {

    }

}
