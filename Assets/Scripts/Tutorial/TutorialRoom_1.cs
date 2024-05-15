using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRoom_1 : MonoBehaviour
{
    [SerializeField] private ClearTutorialRoom tutorialRoom;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialRoom.PointUp();
            gameObject.SetActive(false);
        }
    }
}
