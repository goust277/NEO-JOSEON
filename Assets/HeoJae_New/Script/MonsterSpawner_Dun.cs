using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner_Dun : MonoBehaviour
{
    [Header("���� / ����")]
    public GameObject Monsters;
    public ParticleSystem particleFlash;

    private void Awake()
    {
        StartCoroutine(CreateMonster());
    }

    IEnumerator CreateMonster()
    {
        yield return new WaitForSeconds(1.8f);

        Instantiate(Monsters, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
