using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive (true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
