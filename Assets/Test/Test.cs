using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour, IDamageable
{
    public float moveSpeed = 5.0f;

    public void TakeDamage(Damage damage)
    {
        Debug.Log("HIT");
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            gameObject.transform.Translate(Vector3.forward * dt * moveSpeed);
        if (Input.GetKey(KeyCode.W))
            gameObject.transform.Translate(Vector3.forward * -1 * dt * moveSpeed);
        if (Input.GetKey(KeyCode.D))
            gameObject.transform.Translate(Vector3.right * -1 * dt * moveSpeed);
        if (Input.GetKey(KeyCode.A))
            gameObject.transform.Translate(Vector3.right * dt * moveSpeed);

        Vector3 rot = Vector3.zero;
        rot.y = transform.rotation.eulerAngles.y;
        rot.y += Input.GetAxis("Mouse X") * 5f;
        transform.rotation = Quaternion.Euler(rot);
    }
}
