using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public GameObject[] gameObjects;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObjects[0] == null)
        {
            Destroy(gameObject);
        }
    }
}
