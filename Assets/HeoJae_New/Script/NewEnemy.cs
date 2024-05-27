using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemy : MonoBehaviour
{
    private bool hasDied = false;

    [Header("���� ����")]
    public int maxHp;
    public int currentHp;

  
    public virtual void TakeDamage(int damage)
    {

    }

    public virtual void Die()
    {
        if (hasDied) return;  // �̹� ����� ��� ����
        hasDied = true;
    }



}
