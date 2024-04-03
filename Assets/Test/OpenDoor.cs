using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public List<GameObject> gameObjects;
    public GameObject Door;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObjects.RemoveAll(item => item == null);

        if (gameObjects.Count == 0)
        {
            Destroy(Door);
        }
    }
}
