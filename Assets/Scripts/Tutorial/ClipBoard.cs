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
    private void Start()
    {
        data_Dialog = CSVReader.Read("Test");
        
    }

    [System.Obsolete]
    private void Update()
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
            }
                
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
