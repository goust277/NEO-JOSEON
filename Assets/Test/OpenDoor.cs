using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public GameObject[] gameObjects;
    public GameObject Door;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObjects.Length == 0)
        {
            Destroy(Door);
        }
    }
}
