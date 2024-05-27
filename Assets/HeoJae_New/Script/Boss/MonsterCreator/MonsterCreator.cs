using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCreator : MonoBehaviour
{
    public NewBoss1 boss;
    public StageManagerAssist stageManager;

    public int iMaxMonsterCnt;
    private int iNowMonsterCnt;
    public GameObject MonsterSpawner;
    public float spawnInterval;  // ���� �����ʸ� �����ϴ� ����

    private void Start()
    {
        iNowMonsterCnt = 0;

        StartCoroutine(SpawnMonsterSpawner());
    }

    private IEnumerator SpawnMonsterSpawner()
    {
        while (true)
        {
            // X, Y�� ���� ������ ������ ��ġ ����
            Vector3 randomPosition = new Vector3(Random.Range(-11.0f, 11.0f), 0, Random.Range(-11.0f, 11.0f));


            if(stageManager.smallNum + iMaxMonsterCnt > iNowMonsterCnt && !(boss.bIsDie))
            {
                // MonsterSpawner ����
                Instantiate(MonsterSpawner, randomPosition, Quaternion.identity);
                iNowMonsterCnt++;
            }

            // ������ ���� ���� ���
            yield return new WaitForSeconds(spawnInterval);

        }
    }


}
