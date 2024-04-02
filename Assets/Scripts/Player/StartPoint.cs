using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    public string startPoint;
    private PlayerMove thePlayer;

    private void Start()
    {
        thePlayer = FindAnyObjectByType<PlayerMove>();


        if (startPoint == thePlayer.currentMapName)
        {
            thePlayer.transform.position = this.transform.position;
        }
    }
}
