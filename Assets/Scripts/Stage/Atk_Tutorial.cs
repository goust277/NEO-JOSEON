using UnityEngine;
using UnityEngine.UI;

public class Atk_Tutorial : MonoBehaviour
{
    [Header("진행 상황")]
    public StageManagerAssist stagemanager;

    [Header("몬스터")]
    [SerializeField] private GameObject[] monsters_1;
    [SerializeField] private GameObject[] monsters_2;

    [Header("스폰위치")]
    [SerializeField] private Transform[] spawnPoint_1;
    [SerializeField] private Transform[] spawnPoint_2;

    [Header("다른 참조")]
    [SerializeField] private ClipBoard clipBoard;
    [SerializeField] private Text txt;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(stagemanager.bigNum == stagemanager.smallNum)
        {
            clipBoard.Clear();
        }

    }

    public void SpawnMonster()
    {
        stagemanager.bigNum = monsters_1.Length;
        stagemanager.smallNum = 0;
        for (int i = 0; i < monsters_1.Length; i++) 
        {
            Instantiate(monsters_1[i], spawnPoint_1[i].position, Quaternion.identity);
        }
    }
    public void SpawnMonster_2()
    {
        stagemanager.bigNum = monsters_2.Length;
        stagemanager.smallNum = 0;
        for (int i = 0; i < monsters_2.Length; i++)
        {
            Instantiate(monsters_2[i], spawnPoint_2[i].position, Quaternion.identity);
        }
    }
}
