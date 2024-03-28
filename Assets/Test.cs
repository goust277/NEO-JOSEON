using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    private void Update()
    {
        float dt = Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow))
            gameObject.transform.Translate(Vector3.forward * dt * moveSpeed);
        if (Input.GetKey(KeyCode.UpArrow))
            gameObject.transform.Translate(Vector3.forward * -1 * dt * moveSpeed);
        if (Input.GetKey(KeyCode.RightArrow))
            gameObject.transform.Translate(Vector3.right * -1 * dt * moveSpeed);
        if (Input.GetKey(KeyCode.LeftArrow))
            gameObject.transform.Translate(Vector3.right * dt * moveSpeed);
    }
}
