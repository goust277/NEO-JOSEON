using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1Button : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("Stage1_Test");
    }
}
