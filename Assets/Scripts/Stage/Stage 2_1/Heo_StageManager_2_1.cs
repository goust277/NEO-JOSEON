using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Heo_StageManager_2_1 : MonoBehaviour
{
    [Header("진행 상황")]
    public StageManagerAssist stagemanager;

    [Header("몬스터 종류")]
    public GameObject MonsterSangmo;
    public GameObject MonsterBuoy;
    public GameObject MonsterSpear;
    public GameObject MonsterBow;

    [Header("중간 문들")]
    public GameObject middleDoor_1;
    public GameObject middleDoor_2;

    [Header("스폰 위치")]
    public Transform[] spawnPoint_1;
    public Transform[] spawnPoint_2;

    [Header("물")]
    public int bNowWaterWave; // 1 = 1단계  / 2 = 2단계  / 3 = 클리어
    public float rotationSpeed = 10f;
    public GameObject Water_1;
    public GameObject Water_2;
    public GameObject FloatingObg_1;
    public GameObject FloatingObg_2;
    public WWWater waterScript_1;
    public WWWater waterScript_2;

    private float timer = 0f;
    private int state;
    public bool bWaterCanRotate;
    public float riseSpeed = 1f;


    private void Update()
    {
        // #. 물 회전 관련
        if (bWaterCanRotate && bNowWaterWave == 1)
        {
            WaterRotate_1();
        }
        else if (bWaterCanRotate && bNowWaterWave == 2)
        {
            WaterRotate_2();
        }



        // #. 몬스터 스폰 관련

        if (stagemanager.smallNum >= 6 && stagemanager.bigNum == 0)
        {
            stagemanager.bigNum++;
            stagemanager.smallNum = 0;

            StartCoroutine(Stage1Clear());
        }

        if (stagemanager.smallNum >= 5 && stagemanager.bigNum == 1)
        {
            stagemanager.bigNum++;
            stagemanager.smallNum = 0;

            StartCoroutine(Stage2Clear());

        }


    }


    public void MonsterSpawn_0()
    {
        bWaterCanRotate = true;
        StartRising(-8.87f, Water_1);

        Instantiate(MonsterBuoy, spawnPoint_1[0].position, Quaternion.identity);
        Instantiate(MonsterBuoy, spawnPoint_1[1].position, Quaternion.identity);
        Instantiate(MonsterBuoy, spawnPoint_1[2].position, Quaternion.identity);
        Instantiate(MonsterBuoy, spawnPoint_1[3].position, Quaternion.identity);
        Instantiate(MonsterBow, spawnPoint_1[4].position, Quaternion.identity);
        Instantiate(MonsterBow, spawnPoint_1[5].position, Quaternion.identity);

    }

    public void MonsterSpawn_1()
    {
        bNowWaterWave++;
        bWaterCanRotate = true;
        StartRising(-8.87f, Water_2);

        middleDoor_1.SetActive(true);

        Instantiate(MonsterSangmo, spawnPoint_2[0].position, Quaternion.identity);
        Instantiate(MonsterSangmo, spawnPoint_2[1].position, Quaternion.identity);
        Instantiate(MonsterSangmo, spawnPoint_2[2].position, Quaternion.identity);
        Instantiate(MonsterBow, spawnPoint_2[3].position, Quaternion.identity);
        Instantiate(MonsterBow, spawnPoint_2[4].position, Quaternion.identity);
    }


    private void WaterRotate_1()
    {
        timer += Time.deltaTime;

        switch (state)
        {

            case 0: // 왼쪽 회전
                waterScript_1.bIsTurn = false;
                waterScript_1.waveType = 0;
                if (timer >= 7f)
                {
                    timer = 0f;
                    state = 1;
                }
                break;

            case 1: // 정지
                Water_1.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
                FloatingObg_1.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
                waterScript_1.bIsTurn = true;
                waterScript_1.waveType = 2;
                if (timer >= 7f)
                {
                    timer = 0f;
                    state = 2;
                }
                break;

            case 2: // 오른쪽 회전
                waterScript_1.bIsTurn = false;
                waterScript_1.waveType = 0;
                if (timer >= 7f)
                {
                    timer = 0f;
                    state = 3;
                }
                break;

            case 3: // 정지
                Water_1.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                FloatingObg_1.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                waterScript_1.bIsTurn = true;
                waterScript_1.waveType = 1;
                if (timer >= 7f)
                {
                    timer = 0f;
                    state = 0;
                }
                break;
        }
    }

    private void WaterRotate_2()
    {
        timer += Time.deltaTime;

        switch (state)
        {
            case 0: // 왼쪽 회전
                waterScript_2.bIsTurn = false;
                waterScript_2.waveType = 0;
                if (timer >= 7f)
                {
                    timer = 0f;
                    state = 1;
                }
                break;

            case 1: // 정지
                Water_2.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
                FloatingObg_2.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
                waterScript_2.bIsTurn = true;
                waterScript_2.waveType = 2;
                if (timer >= 7f)
                {
                    timer = 0f;
                    state = 2;
                }
                break;

            case 2: // 오른쪽 회전
                waterScript_2.bIsTurn = false;
                waterScript_2.waveType = 0;
                if (timer >= 7f)
                {
                    timer = 0f;
                    state = 3;
                }
                break;

            case 3: // 정지
                Water_2.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                FloatingObg_2.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                waterScript_2.bIsTurn = true;
                waterScript_2.waveType = 1;
                if (timer >= 7f)
                {
                    timer = 0f;
                    state = 0;
                }
                break;
        }
    }


    public void StartRising(float targetYPosition, GameObject water)
    {
        StartCoroutine(MoveToPosition(targetYPosition, water));
    }

    public void StartLowering(float targetYPosition, GameObject water)
    {
        StartCoroutine(MoveToPosition(targetYPosition, water));
    }

    private IEnumerator MoveToPosition(float targetYPosition, GameObject water)
    {
        Vector3 startPosition = water.transform.position;
        Vector3 endPosition = new Vector3(startPosition.x, targetYPosition, startPosition.z);

        while (Mathf.Abs(water.transform.position.y - targetYPosition) > 0.01f)
        {
            water.transform.position = Vector3.MoveTowards(water.transform.position, endPosition, riseSpeed * Time.deltaTime);
            yield return null;
        }

        water.transform.position = endPosition;
    }


    private IEnumerator Stage1Clear()
    {
        state = 0;
        bWaterCanRotate = false;
        StartLowering(-15f, Water_1);

        yield return new WaitForSeconds(2f);

        middleDoor_1.SetActive(false);
    }

    private IEnumerator Stage2Clear()
    {
        state = 0;
        bWaterCanRotate = false;
        StartLowering(-15f, Water_2);

        yield return new WaitForSeconds(2f);

        OpenNextStage();
    }
    private void OpenNextStage()
    {
        middleDoor_2.SetActive(false);
    }
}
