using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InGameManager : MonoBehaviour
{
    private GameObject player;
    private float dashCoolTime;
    private float dashDelay;

    [Header("아이콘")]
    [SerializeField] private Image skillImg;
    [SerializeField] private Image dashImg;

    [Header("쿨 타임 표시")]
    [SerializeField] private Text skillTxt;
    [SerializeField] private Text dashTxt;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
    }

    // Update is called once per frame
    void Update()
    {
        dashCoolTime = player.GetComponent<PlayerMove>().dashCoolTime;
        dashDelay = player.GetComponent<PlayerMove>().dashDelay;

        dashImg.fillAmount = dashDelay / dashCoolTime;

        if (dashDelay <= dashCoolTime)
        {
            int cool = (int)dashCoolTime - (int)dashDelay;

            dashTxt.text = cool.ToString();
        }
        else
        {
            dashTxt.text = null;
        }
    }
}
