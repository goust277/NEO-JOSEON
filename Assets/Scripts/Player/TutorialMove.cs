using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialMove : MonoBehaviour
{
    [SerializeField] private GameObject letter;

    // Update is called once per frame
    void Update()
    {
        if (letter == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                letter.SetActive(false);
            }
        }

    }
}
