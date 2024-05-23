using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageButton : MonoBehaviour
{
    [SerializeField] private string SceneName;
    private GameObject mainSetting;
    private GameObject stageSetting;

    private void Awake()
    {
        mainSetting = GameObject.Find("Main_Setting");
        stageSetting = GameObject.Find("Stage_Setting");
    }
    public void OnClick()
    {
        SceneManager.LoadScene(SceneName);
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StageExit()
    {
        SceneManager.LoadScene("Stage_Feild");
    }
    public void ExitUI()
    {
        if (SceneManager.GetActiveScene().name == "Stage_Feild")
        {
            mainSetting.GetComponent<MainSetting>().Close();
        }
        else
        {
            stageSetting.GetComponent<StageSetting>().Close();
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
