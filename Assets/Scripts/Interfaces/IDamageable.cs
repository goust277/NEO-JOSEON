using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Damage
{
    public float amount;
    public string property;
}

public interface IDamageable
{
    public void TakeDamage(Damage damage);
}
