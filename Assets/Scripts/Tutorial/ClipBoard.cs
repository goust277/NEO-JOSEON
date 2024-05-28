using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class ClipBoard : MonoBehaviour
{

    [Header("대사 오브젝트")]
    [SerializeField] private GameObject txtui;
    [SerializeField] private Text text;

    [Header("플레이어")]
    [SerializeField] private PlayerMove playerMove;

    [Header("편지")]
    [SerializeField] private GameObject letter;

    [Header("다른 참조")]
    public int id = 0;
    List<Dictionary<string, object>> data_Dialog;

    [Header("기능")]
    [SerializeField] private GameObject wasd;
    [SerializeField] private GameObject space;
    [SerializeField] private GameObject shift;
    private void Start()
    {
        data_Dialog = CSVReader.Read("Test");
        wasd.SetActive(false);
        space.SetActive(false);
        shift.SetActive(false);


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
