using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DmgNum : MonoBehaviour
{
    public Text text_dmgNum;
    public int dmgNum;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
       

        Vector3 randomDirection = Random.insideUnitSphere.normalized;

        randomDirection.y = 0f;

        rb.AddForce(randomDirection * 3f, ForceMode.Impulse);
        rb.AddForce(Vector3.up * 3f, ForceMode.Impulse);
    }
}
