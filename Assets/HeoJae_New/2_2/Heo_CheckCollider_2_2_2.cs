using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heo_CheckCollider_2_2_2 : MonoBehaviour
{
    public Heo_StageManager_2_2 stageManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stageManager.MonsterSpawn_1();
            Destroy(gameObject);
        }
    }
}
