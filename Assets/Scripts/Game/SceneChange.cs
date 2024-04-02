using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField] private string SceneName;

    private PlayerMove thePlayer;
    private void Awake()
    {
        thePlayer = FindObjectOfType<PlayerMove>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            thePlayer.currentMapName = SceneName;
            SceneManager.LoadScene(SceneName);
        }
    }
}
