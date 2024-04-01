using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{ 
    private static Game instance = null;
    private SceneControl sceneControl = null;

    public static Game Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject newGO = new GameObject();
                newGO.name = "[ GameControl ]";
                DontDestroyOnLoad(newGO);

                instance = newGO.AddComponent<Game>();
                instance.sceneControl = newGO.AddComponent<SceneControl>();
            } 
            return instance;
        }
    }
    public SceneControl SceneControl
    { get { return sceneControl; } }
}
