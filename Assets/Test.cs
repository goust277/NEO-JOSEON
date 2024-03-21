using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour, IDamageable
{
    public void TakeDamage(Damage damage)
    {
        Destroy(gameObject);
    }
}
