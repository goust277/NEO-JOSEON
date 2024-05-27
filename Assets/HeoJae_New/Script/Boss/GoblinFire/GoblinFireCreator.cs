using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GoblinFireCreator : MonoBehaviour
{
    public NewBoss1 boss;

    [Header("�������")]
    public GameObject objGoblinFire;

    [Header("��ƼŬ")]
    public ParticleSystem particleGoblinFire;

    [Header("���� ��ġ")]
    public Transform[] createPositionLine_1;
    public Transform[] createPositionLine_2;
    public Transform[] createPositionLine_3;
    public Transform[] createPositionLine_4;

    [Header("���� �ֱ�")]
    public float interval = 3f;
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval && !(boss.bIsDie))
        {
            CreateGoblinFire();
            timer = 0f;
        }
    }

    private void CreateGoblinFire()
    {
        int ranNum = Random.Range(1, 5);
        int ranNumPosition = Random.Range(0, 20);
        switch (ranNum)
        {
            case 1:
                StartCoroutine(SpawnParticles(createPositionLine_1[ranNumPosition], ranNum));
                break;
            case 2:
                StartCoroutine(SpawnParticles(createPositionLine_2[ranNumPosition], ranNum));
                break;
            case 3:
                StartCoroutine(SpawnParticles(createPositionLine_3[ranNumPosition], ranNum));
                break;
            case 4:
                StartCoroutine(SpawnParticles(createPositionLine_4[ranNumPosition], ranNum));
                break;

        }
    }

    IEnumerator SpawnParticles(Transform spawnPoint,int num)
    {
        Debug.Log("������");
        float timer = 0f;

        while (timer < 2f)
        {
            Quaternion rotation = Quaternion.identity;

            if (num == 1 || num == 2) rotation *= Quaternion.Euler(90f, 0f, 0f);
            if (num == 3 || num == 4) rotation *= Quaternion.Euler(90f, 0f, 90f);

            Instantiate(particleGoblinFire, spawnPoint.position, rotation); // ��ƼŬ ����
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }
        Debug.Log("������� ����");
        Vector3 spawnPosition = spawnPoint.position;
        spawnPosition.y -= 3f; // y������ -3 �̵�
        GameObject mover = Instantiate(objGoblinFire, spawnPosition, Quaternion.identity);
        GoblinFireMover moverScript = mover.GetComponent<GoblinFireMover>();
        moverScript.DirectionSetting(num);
    }













}
