using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heo_CheckCollider_1_2_1 : MonoBehaviour
{
    public Heo_StageManager_1_2 stageManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stageManager.MonsterSpawn_0();
            Destroy(gameObject);
        }
    }
}
