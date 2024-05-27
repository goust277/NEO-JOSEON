using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("몬스터 인덱스")]
    public int monsterIndex;

    [Header("몬스터 / 프랍")]
    public GameObject[] Monsters;
    public ParticleSystem particleFlash;

    private NewBoss1 boss;

    private void Awake()
    {
        boss = FindObjectOfType<NewBoss1>();

        if (boss != null) Debug.Log("Boss found: " + boss.name);
        else Debug.Log("No boss found in the scene.");

        StartCoroutine(CreateMonster());
    }

    IEnumerator CreateMonster()
    {
        monsterIndex = Random.Range(0, 2);

        yield return new WaitForSeconds(1.8f);

        if (!(boss.bIsDie)) Instantiate(Monsters[monsterIndex], transform.position, Quaternion.identity);

        Destroy(gameObject);
    }


}
