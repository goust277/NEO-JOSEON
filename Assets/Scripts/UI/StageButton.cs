using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageButton : MonoBehaviour
{
    [SerializeField] private string SceneName;
    private GameObject mainSetting;
    private GameObject stageSetting;
    [SerializeField] private GameObject UI;
    private GameObject freeLookCamera;
    private GameObject player;
    private void Awake()
    {
        player = GameObject.Find("Player");
        mainSetting = GameObject.Find("Main_Setting");
        stageSetting = GameObject.Find("Stage_Setting");
        freeLookCamera = GameObject.FindGameObjectWithTag("FLCamera");
    }
    public void OnClick()
    {
        SceneManager.LoadScene(SceneName);
    }

    public void MainStart()
    {
        if (GameData.clearTuto == false)
        {
            SceneManager.LoadScene(SceneName);
        }
        else if (GameData.clearTuto == true)
        {
            SceneManager.LoadScene("Stage_Felid_New");
        }
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StageExit()
    {
        SceneManager.LoadScene("Stage_Felid_New");
    }
    public void ExitUI()
    {
        if (SceneManager.GetActiveScene().name == "Stage_Feild_New")
        {
            mainSetting.GetComponent<MainSetting>().Close();
            freeLookCamera.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            stageSetting.GetComponent<StageSetting>().Close();
            freeLookCamera.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Close()
    {
        if (UI != null)
        {
            UI.SetActive(false);
            freeLookCamera.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            player.GetComponent<PlayerMove>().CloseSetting();
        }
    }

    public void MainClose()
    {
        if (UI != null)
        {
            UI.SetActive(false);
        }
    }

    public void MainOpen()
    {
        if (UI != null)
        {
            UI.SetActive(true);
        }
    }
}
