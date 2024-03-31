using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Damage : MonoBehaviour
{
    [SerializeField] private int hp;
    public GameObject enemy;

    private void Update()
    {
        if (hp == 0)
        {
            Destroy(enemy);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Melee"))
        {
            Weapon weapon = other.gameObject.GetComponent<Weapon>();
            hp -= weapon.damage;
            Debug.Log("MeleeAttack :" + hp);
        }
    }

}
