using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBar : MonoBehaviour
{
    private Image img;
    [SerializeField] private GameObject player;
    void Start()
    {
        img = GetComponent<Image>();
        player = GameObject.FindGameObjectWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
            img.fillAmount = player.GetComponent<PlayerDamage>().CurrentHp / player.GetComponent<PlayerDamage>().MaxHp;
    }
}
