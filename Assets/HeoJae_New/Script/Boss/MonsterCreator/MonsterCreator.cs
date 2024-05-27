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
    public float spawnInterval;  // 몬스터 스포너를 생성하는 간격

    private void Start()
    {
        iNowMonsterCnt = 0;

        StartCoroutine(SpawnMonsterSpawner());
    }

    private IEnumerator SpawnMonsterSpawner()
    {
        while (true)
        {
            // X, Y축 범위 내에서 무작위 위치 생성
            Vector3 randomPosition = new Vector3(Random.Range(-11.0f, 11.0f), 0, Random.Range(-11.0f, 11.0f));


            if(stageManager.smallNum + iMaxMonsterCnt > iNowMonsterCnt && !(boss.bIsDie))
            {
                // MonsterSpawner 생성
                Instantiate(MonsterSpawner, randomPosition, Quaternion.identity);
                iNowMonsterCnt++;
            }

            // 지정된 간격 동안 대기
            yield return new WaitForSeconds(spawnInterval);

        }
    }


}
