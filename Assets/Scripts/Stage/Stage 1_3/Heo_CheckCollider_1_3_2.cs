using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heo_CheckCollider_1_3_2 : MonoBehaviour
{
    public Heo_StageManager_1_3 stageManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stageManager.MonsterSpawn_1();
            Destroy(gameObject);
        }
    }
}
