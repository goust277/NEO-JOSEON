using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterArrow : MonoBehaviour
{
    public float moveSpeed;

    void Update()
    {
        transform.Translate(transform.forward * Time.deltaTime * moveSpeed, Space.World);
    }

}
