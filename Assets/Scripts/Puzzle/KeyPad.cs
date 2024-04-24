using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KeyPad : MonoBehaviour
{
    public GameObject DoorObj;

    [SerializeField]private int openKey;
    [SerializeField] private int open = 0;
    public int notOpen = 0;
    private bool close = false;
    void Start()
    {
        
    }

    void Update()
    {
        if (open == openKey && notOpen == 0)
        {
            DoorObj.SetActive(false);
        }
    }

    public void OpenCancle()
    {
        open--;
    }
    public void AddOpen()
    {
        open += 1;
    }

    public void CloseDoor()
    {
        close = true;
    }

    public void Clear()
    {
        open = 0;
        close = false;
    }
}
