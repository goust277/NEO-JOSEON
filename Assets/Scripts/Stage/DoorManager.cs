using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [SerializeField] private GameObject door;

    [SerializeField] private int targetKill;
    [SerializeField] private int killCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (killCount == targetKill)
        {
            Destroy(door);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("Enemy"))
        //{
            if (other.gameObject == null)
            {
                killCount++;
            }
        //}
    }
}
