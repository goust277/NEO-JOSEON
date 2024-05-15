using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallPlayer : MonoBehaviour
{
    [SerializeField] private GameObject setpoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = setpoint.transform.position;
        }

    }
}
