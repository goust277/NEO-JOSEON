using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class ClipBoard : MonoBehaviour
{
    [Header("��� ������Ʈ")]
    [SerializeField] private GameObject txtui;
    [SerializeField] private Text text;

    [Header("�÷��̾�")]
    [SerializeField] private PlayerMove playerMove;

    [Header("����")]
    [SerializeField] private GameObject letter;

    [Header("�ٸ� ����")]
    public int id = 0;
    List<Dictionary<string, object>> data_Dialog;
    [SerializeField] private string csvName;
    [SerializeField] private Atk_Tutorial tuto;

    [Header("���")]
    [SerializeField] private GameObject wasd;
    [SerializeField] private GameObject space;
    [SerializeField] private GameObject shift;
    private void Start()
    {
        data_Dialog = CSVReader.Read(csvName);
        wasd.SetActive(false);
        space.SetActive(false);
        shift.SetActive(false);


    }

    [System.Obsolete]
    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "tutorial")
        {
            if (txtui.active == true && letter.active == false)
            {
                if (playerMove != null)
                {
                    playerMove.onTxt = true;
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    id++;
                }
                Debug.Log("d");
                text.text = data_Dialog[id]["Content"].ToString();

                if (id == 10 || id == 14 || id == 17)
                {
                    if (playerMove != null)
                    {
                        playerMove.onTxt = false;
                    }
                    txtui.SetActive(false);
                    playerMove.MouseOff();
                }

            }
            if (id == 10)
                wasd.SetActive(true);
            else
                wasd.SetActive(false);
            if (id == 14)
                space.SetActive(true);
            else
                space.SetActive(false);
            if (id == 17)
                shift.SetActive(true);
            else
                shift.SetActive(false);
        }

        else if (SceneManager.GetActiveScene().name == "tutorial_atk")
        {
            if (txtui.active == true && letter.active == false)
            {
                if (playerMove != null)
                {
                    playerMove.onTxt = true;
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    id++;
                }
                Debug.Log("d");
                text.text = data_Dialog[id]["Content"].ToString();

                if (id == 5 || id == 10 || id == 14)
                {
                    if (playerMove != null)
                    {
                        playerMove.onTxt = false;
                    }
                    txtui.SetActive(false);
                    playerMove.MouseOff();
                    tuto.SpawnMonster();
                }
            }
            if (id == 5)
                wasd.SetActive(true);
            else
                wasd.SetActive(false);
            if (id == 10)
                space.SetActive(true);
            else
                space.SetActive(false);
        }
    }
    public void Clear()
    {
        if (playerMove != null)
        {
            playerMove.onTxt = true;
        }
        id++;
        txtui.SetActive (true);
    }
}
