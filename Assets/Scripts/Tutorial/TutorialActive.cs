using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialActive : MonoBehaviour
{
    [SerializeField] private ClipBoard clipboard;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMove>().ClearTutorial();
            gameObject.SetActive(false);

            if(clipboard != null) 
            {
                clipboard.Clear();
            }
        }
    }
}
