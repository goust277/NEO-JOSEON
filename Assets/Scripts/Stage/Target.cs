using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private GameObject target;

    private void Update()
    {
        Vector3 dir = target.transform.position - transform.position;
        dir.y = 0f;

        Quaternion rot = Quaternion.LookRotation(-dir.normalized);

        transform.rotation = rot;
    }
}
