using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTest : MonoBehaviour, IDamageable
{
    public void TakeDamage(Damage damage)
    {
        Destroy(gameObject);
    }
}
