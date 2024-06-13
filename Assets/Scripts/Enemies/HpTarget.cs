using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpTarget : MonoBehaviour
{
    [SerializeField] private GameObject target;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = transform.position - target.transform.position;
        dir.y = 0f;

        Quaternion rot = Quaternion.LookRotation(dir.normalized);

        transform.rotation = rot;
    }
}
