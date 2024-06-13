using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private string csvName;
    [SerializeField] private Atk_Tutorial tuto;

    [Header("기능")]
    [SerializeField] private GameObject wasd;
    [SerializeField] private GameObject space;
    [SerializeField] private GameObject shift;

    [Header("문")]
    [SerializeField] private GameObject[] door;

    private bool closeletter = false;
    private bool mouseOff = false;
    private void Start()
    {
        data_Dialog = CSVReader.Read(csvName);
        if (wasd != null  )
        {
            wasd.SetActive(false);
        }
        if (space != null )
        {
            space.SetActive(false);
        }
        if (shift != null)
        {
            shift.SetActive(true);
        }


    }

    [System.Obsolete]
    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "tutorial")
        {
            if (letter.active)
            {
                playerMove.onTxt = true;
            }
            else if (letter.active == false && closeletter == false)
            {
                txtui.SetActive(true);
                closeletter = false;
            }
            if (txtui.active == true )
            {
                if (playerMove != null)
                {
                    playerMove.onTxt = true;
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (id == 10 || id == 14 || id == 17)
                    {

                    }
                    else
                    {
                        mouseOff = false;
                        id++;
                    }
                        
                }
                text.text = data_Dialog[id]["Content"].ToString();

                if (id == 10 || id == 14 || id == 17)
                {
                    if (playerMove != null)
                    {
                        playerMove.onTxt = false;
                    }
                    txtui.SetActive(false);

                    if (mouseOff == false)
                    {
                        playerMove.MouseOff();
                        mouseOff = true;
                    }
                   
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

        if (SceneManager.GetActiveScene().name == "tutorial _atk")
        {
            if (txtui.active == true)
            {
                if (playerMove != null)
                {
                    playerMove.onTxt = true;
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    id++;
                }
                text.text = data_Dialog[id]["Content"].ToString();

                if (id == 5 || id == 9 || id == 13)
                {
                    if (playerMove != null)
                    {
                        playerMove.onTxt = false;
                    }
                    txtui.SetActive(false);

                    playerMove.MouseOff();
                    playerMove.ClearAtkTutorial();
                    if(id == 5)
                    {
                        tuto.SpawnMonster();
                    }
                    if (id == 9)
                    {
                        tuto.SpawnMonster_2();
                    }
                    if (id == 13)
                    {
                        for (int i = 0; i < door.Length; i++) 
                        {
                            door[i].SetActive(false);
                        }
                    }
                    
                }
            }
            if (id == 5)
            {
                wasd.SetActive(true);
            }
                
            else
                wasd.SetActive(false);
            if (id == 9)
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
