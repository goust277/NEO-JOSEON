using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private GameObject screenEffect = null;
    private static Game instance = null;

    private SceneControl sceneControl = null;
    private HPbar hpbar = null;

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
                GameObject prefab = Resources.Load<GameObject>("Prefab/System/ScreenEffect");
                instance.screenEffect = Instantiate(prefab);
                DontDestroyOnLoad(instance.screenEffect);

                instance.sceneControl = newGO.AddComponent<SceneControl>();
                instance.sceneControl.Initialize(instance.screenEffect);

                instance.hpbar = newGO.AddComponent<HPbar>();
                instance.hpbar.Initialize(instance.screenEffect);
            } 
            return instance;
        }
    }
    public SceneControl SceneControl
    { get { return sceneControl; } }

    public HPbar HPbar
    {
        get { return hpbar; }
    }
}
