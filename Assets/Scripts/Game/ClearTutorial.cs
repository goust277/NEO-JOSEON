using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearTutorial : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameData.TutoClear();
    }
}
