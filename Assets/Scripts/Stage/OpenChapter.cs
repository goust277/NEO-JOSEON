using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenChapter : MonoBehaviour
{
    [Header("°Ç¹°")]
    [SerializeField] private GameObject[] buildings;

    // Update is called once per frame
    void Update()
    {
        if (GameData.Chapter >= 2)
        {
            for (int i = 0; i < buildings.Length; i++) 
            {
                buildings[i].gameObject.SetActive(false);
            }
        }
    }
}
