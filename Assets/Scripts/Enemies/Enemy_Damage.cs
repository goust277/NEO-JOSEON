using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Damage : MonoBehaviour
{
    [SerializeField] private int hp;
    [SerializeField] private Material[] materials;
    
    public GameObject enemy;

    private Renderer rend;

    private void Awake()
    {
        rend = enemy.GetComponent<Renderer>();
    }
    private void Update()
    {
        if (hp <= 0)
        {
            Destroy(enemy);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Melee"))
        {
            rend.material = materials[1];
            Weapon weapon = other.gameObject.GetComponent<Weapon>();
            hp -= weapon.damage;
            Debug.Log("MeleeAttack :" + hp);
            Invoke("Idle", 0.5f);
        }
    }

    private void Idle()
    {
        rend.material = materials[0];
    }
}
