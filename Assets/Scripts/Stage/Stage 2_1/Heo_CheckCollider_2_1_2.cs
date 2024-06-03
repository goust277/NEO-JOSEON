using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heo_CheckCollider_2_1_2 : MonoBehaviour
{
    public Heo_StageManager_2_1 stageManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stageManager.MonsterSpawn_1();
            Destroy(gameObject);
        }
    }
}
