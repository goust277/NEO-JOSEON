using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialActive : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMove>().ClearTutorial();
            gameObject.SetActive(false);
        }
    }
}
